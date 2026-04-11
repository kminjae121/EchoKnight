using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Map
{
    public static class MapGenerator
    {
        private static MapConfigSO _config;
        private static List<List<MapNode>> _nodes = new List<List<MapNode>>();

        public static MapData GenerateMap(MapConfigSO config)
        {
            if (config == null)
            {
                Debug.LogError("맵 설정 파일(MapConfig)이 없습니다.");
                return null;
            }

            _config = config;
            _nodes.Clear();

            GenerateGrid();
            List<List<Vector2Int>> paths = GeneratePaths();
            SetUpConnections(paths);
            RemoveCrossConnections();

            List<MapNode> finalNodes = _nodes.SelectMany(n => n)
                .Where(n => !n.HasNoConnections()).ToList();

            AssignNodeTypes(finalNodes);
            RandomizeNodePositions(finalNodes);

            MapData mapData = new MapData();
            mapData.configName = config.name;
            mapData.nodes = finalNodes;

            return mapData;
        }

        private static void GenerateGrid()
        {
            float startX = -((_config.gridWidth - 1) * _config.nodeSpacingX) / 2f;
            
            for (int y = 0; y < _config.numOfLayers; y++)
            {
                List<MapNode> layerNodes = new List<MapNode>();
                for (int x = 0; x < _config.gridWidth; x++)
                {
                    MapNode node = new MapNode(new Vector2Int(x, y));
                    node.position = new Vector2(startX + (x * _config.nodeSpacingX), y * _config.nodeSpacingY);
                    layerNodes.Add(node);
                }
                _nodes.Add(layerNodes);
            }
        }

        private static List<List<Vector2Int>> GeneratePaths()
        {
            Vector2Int finalNode = GetFinalNode();
            var paths = new List<List<Vector2Int>>();
            
            List<int> candidateXs = new List<int>();
            for (int i = 0; i < _config.gridWidth; i++) candidateXs.Add(i);

            Shuffle(candidateXs);
            List<Vector2Int> startingPoints = candidateXs.Take(_config.numOfStartingNodes).Select(x => new Vector2Int(x, 0)).ToList();

            Shuffle(candidateXs);
            List<Vector2Int> preBossPoints = candidateXs.Take(_config.numOfPreBossNodes).Select(x => new Vector2Int(x, finalNode.y - 1)).ToList();

            int numOfPaths = Mathf.Max(_config.numOfStartingNodes, _config.numOfPreBossNodes) + Mathf.Max(0, _config.extraPaths);
            
            for (int i = 0; i < numOfPaths; ++i)
            {
                Vector2Int startNode = startingPoints[i % _config.numOfStartingNodes];
                Vector2Int endNode = preBossPoints[i % _config.numOfPreBossNodes];
                
                List<Vector2Int> path = CreatePath(startNode, endNode);
                path.Add(finalNode);
                paths.Add(path);
            }

            return paths;
        }

        private static List<Vector2Int> CreatePath(Vector2Int fromPoint, Vector2Int toPoint)
        {
            int toRow = toPoint.y;
            int toCol = toPoint.x;
            int lastNodeCol = fromPoint.x;

            List<Vector2Int> path = new List<Vector2Int> { fromPoint };
            List<int> candidateCols = new List<int>();
            
            for (int row = 1; row < toRow; ++row)
            {
                candidateCols.Clear();
                int verticalDistance = toRow - row;

                if (Mathf.Abs(toCol - lastNodeCol) <= verticalDistance)
                    candidateCols.Add(lastNodeCol);

                int leftCol = lastNodeCol - 1;
                if (leftCol >= 0 && Mathf.Abs(toCol - leftCol) <= verticalDistance)
                    candidateCols.Add(leftCol);

                int rightCol = lastNodeCol + 1;
                if (rightCol < _config.gridWidth && Mathf.Abs(toCol - rightCol) <= verticalDistance)
                    candidateCols.Add(rightCol);

                int candidateCol = candidateCols[Random.Range(0, candidateCols.Count)];
                path.Add(new Vector2Int(candidateCol, row));
                lastNodeCol = candidateCol;
            }

            path.Add(toPoint);
            return path;
        }

        private static void SetUpConnections(List<List<Vector2Int>> paths)
        {
            foreach (List<Vector2Int> path in paths)
            {
                for (int i = 0; i < path.Count - 1; ++i)
                {
                    MapNode node = GetNode(path[i]);
                    MapNode nextNode = GetNode(path[i + 1]);
                    node.AddOutgoing(nextNode.point);
                    nextNode.AddIncoming(node.point);
                }
            }
        }

        private static void RemoveCrossConnections()
        {
            for (int i = 0; i < _config.gridWidth - 1; ++i)
            {
                for (int j = 0; j < _config.numOfLayers - 1; ++j)
                {
                    MapNode node = GetNode(new Vector2Int(i, j));
                    if (node == null || node.HasNoConnections()) continue;
                    
                    MapNode right = GetNode(new Vector2Int(i + 1, j));
                    if (right == null || right.HasNoConnections()) continue;
                    
                    MapNode top = GetNode(new Vector2Int(i, j + 1));
                    if (top == null || top.HasNoConnections()) continue;
                    
                    MapNode topRight = GetNode(new Vector2Int(i + 1, j + 1));
                    if (topRight == null || topRight.HasNoConnections()) continue;

                    if (!node.outgoing.Contains(topRight.point)) continue;
                    if (!right.outgoing.Contains(top.point)) continue;

                    node.AddOutgoing(top.point);
                    top.AddIncoming(node.point);

                    right.AddOutgoing(topRight.point);
                    topRight.AddIncoming(right.point);

                    float rnd = Random.Range(0f, 1f);
                    if (rnd < 0.2f)
                    {
                        node.RemoveOutgoing(topRight.point);
                        topRight.RemoveIncoming(node.point);
                        right.RemoveOutgoing(top.point);
                        top.RemoveIncoming(right.point);
                    }
                    else if (rnd < 0.6f)
                    {
                        node.RemoveOutgoing(topRight.point);
                        topRight.RemoveIncoming(node.point);
                    }
                    else
                    {
                        right.RemoveOutgoing(top.point);
                        top.RemoveIncoming(right.point);
                    }
                }
            }
        }

        private static MapNode GetNode(Vector2Int p)
        {
            if (p.y >= _nodes.Count) return null;
            if (p.x >= _nodes[p.y].Count) return null;
            return _nodes[p.y][p.x];
        }

        private static Vector2Int GetFinalNode()
        {
            int y = _config.numOfLayers - 1;
            
            if (_config.gridWidth % 2 == 1)
                return new Vector2Int(_config.gridWidth / 2, y);

            return Random.Range(0, 2) == 0
                ? new Vector2Int(_config.gridWidth / 2, y)
                : new Vector2Int(_config.gridWidth / 2 - 1, y);
        }

        private static void AssignNodeTypes(List<MapNode> finalNodes)
        {
            foreach (var node in finalNodes)
            {
                int y = node.point.y;

                if (y == 0)
                {
                    node.nodeType = MapNodeType.MinorEnemy;
                }
                else if (y == _config.preBossRestSiteLayer)
                {
                    node.nodeType = MapNodeType.RestSite;
                }
                else if (y == _config.numOfLayers - 1)
                {
                    node.nodeType = MapNodeType.Boss;
                }
                else
                {
                    node.nodeType = GetRandomNodeType(_config);
                    
                    if (node.nodeType == MapNodeType.EliteEnemy && y < _config.minEliteLayer)
                    {
                        node.nodeType = MapNodeType.MinorEnemy;
                    }
                    
                    EnsureNoConsecutiveTypes(node);
                }
            }
        }

        private static void EnsureNoConsecutiveTypes(MapNode node)
        {
            if (node.nodeType == MapNodeType.EliteEnemy || node.nodeType == MapNodeType.RestSite || node.nodeType == MapNodeType.Store)
            {
                foreach (var incomingPoint in node.incoming)
                {
                    var incomingNode = GetNode(incomingPoint);
                    if (incomingNode != null && incomingNode.nodeType == node.nodeType)
                    {
                        node.nodeType = MapNodeType.MinorEnemy;
                        break;
                    }
                }
            }
        }

        private static MapNodeType GetRandomNodeType(MapConfigSO config)
        {
            if (config.defaultBlueprints == null || config.defaultBlueprints.Count == 0)
                return MapNodeType.MinorEnemy;

            float totalWeight = config.defaultBlueprints.Sum(b => b.weight);
            float randomVal = Random.Range(0f, totalWeight);
            float currentWeight = 0;

            foreach (var blueprint in config.defaultBlueprints)
            {
                currentWeight += blueprint.weight;
                if (randomVal <= currentWeight)
                {
                    return blueprint.nodeType;
                }
            }
            return MapNodeType.MinorEnemy;
        }

        private static void RandomizeNodePositions(List<MapNode> finalNodes)
        {
            foreach (var node in finalNodes)
            {
                // 보스 노드(가장 위)는 정중앙을 유지하도록 흔들지 않습니다.
                if (node.nodeType == MapNodeType.Boss) continue;

                float jitterX = Random.Range(-_config.positionJitterX, _config.positionJitterX);
                float jitterY = Random.Range(-_config.positionJitterY, _config.positionJitterY);
                
                node.position += new Vector2(jitterX, jitterY);
            }
        }

        private static void Shuffle<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int rnd = Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[rnd];
                list[rnd] = temp;
            }
        }
    }
}
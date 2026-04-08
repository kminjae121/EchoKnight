using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Map
{
    public static class MapGenerator
    {
        public static MapData GenerateMap(MapConfigSO config)
        {
            if (config == null)
            {
                Debug.LogError("맵 설정 파일(MapConfig)이 없습니다.");
                return null;
            }

            MapData mapData = new MapData();
            mapData.configName = config.name;

            List<List<MapNode>> grid = new List<List<MapNode>>();
            for (int i = 0; i < config.numOfLayers; i++)
            {
                grid.Add(new List<MapNode>());
                for (int j = 0; j < config.gridWidth; j++)
                {
                    grid[i].Add(new MapNode(new Vector2Int(j, i)));
                }
            }

            GeneratePaths(grid, config);
            CullUnreachableNodes(grid);
            AssignPositions(grid, config);
            AssignNodeTypes(grid, config);

            foreach (var layer in grid)
            {
                foreach (var node in layer)
                {
                    if (node.incoming.Count > 0 || node.outgoing.Count > 0 || node.point.y == 0)
                    {
                        mapData.nodes.Add(node);
                    }
                }
            }

            return mapData;
        }

        private static void GeneratePaths(List<List<MapNode>> grid, MapConfigSO config)
        {
            List<int> startNodes = GetRandomIndices(config.gridWidth, config.startingNodesCount);
            
            foreach (int startIndex in startNodes)
            {
                int currentX = startIndex;
                for (int y = 0; y < config.numOfLayers - 1; y++)
                {
                    int nextY = y + 1;
                    List<int> validNextX = GetValidNextNodes(currentX, config.gridWidth);
                    int nextX = validNextX[Random.Range(0, validNextX.Count)];

                    if (!WillCauseCross(grid, currentX, y, nextX, nextY))
                    {
                        grid[y][currentX].outgoing.Add(new Vector2Int(nextX, nextY));
                        grid[nextY][nextX].incoming.Add(new Vector2Int(currentX, y));
                        currentX = nextX;
                    }
                    else
                    {
                        nextX = currentX;
                        grid[y][currentX].outgoing.Add(new Vector2Int(nextX, nextY));
                        grid[nextY][nextX].incoming.Add(new Vector2Int(currentX, y));
                    }
                }
            }
        }

        private static bool WillCauseCross(List<List<MapNode>> grid, int startX, int startY, int endX, int endY)
        {
            foreach (var node in grid[startY])
            {
                foreach (var outgoing in node.outgoing)
                {
                    if (node.point.x < startX && outgoing.x > endX) return true;
                    if (node.point.x > startX && outgoing.x < endX) return true;
                }
            }
            return false;
        }

        private static List<int> GetValidNextNodes(int currentX, int gridWidth)
        {
            List<int> validNodes = new List<int> { currentX };
            if (currentX > 0) validNodes.Add(currentX - 1);
            if (currentX < gridWidth - 1) validNodes.Add(currentX + 1);
            return validNodes;
        }

        private static void CullUnreachableNodes(List<List<MapNode>> grid)
        {
            for (int y = grid.Count - 2; y >= 0; y--)
            {
                foreach (var node in grid[y])
                {
                    if (node.outgoing.Count == 0)
                    {
                        foreach (var incoming in node.incoming)
                        {
                            grid[incoming.y][incoming.x].outgoing.Remove(node.point);
                        }
                        node.incoming.Clear();
                    }
                }
            }
        }

        private static void AssignPositions(List<List<MapNode>> grid, MapConfigSO config)
        {
            float startX = -((config.gridWidth - 1) * config.nodeSpacingX) / 2f;
            float startY = 0f;

            for (int y = 0; y < grid.Count; y++)
            {
                for (int x = 0; x < grid[y].Count; x++)
                {
                    float jitterX = Random.Range(-config.positionJitterX, config.positionJitterX);
                    float jitterY = Random.Range(-config.positionJitterY, config.positionJitterY);
                    
                    grid[y][x].position = new Vector2(
                        startX + (x * config.nodeSpacingX) + jitterX,
                        startY + (y * config.nodeSpacingY) + jitterY
                    );
                }
            }
        }

        private static void AssignNodeTypes(List<List<MapNode>> grid, MapConfigSO config)
        {
            for (int y = 0; y < grid.Count; y++)
            {
                foreach (var node in grid[y])
                {
                    if (node.incoming.Count == 0 && node.outgoing.Count == 0) continue;

                    if (y == 0)
                    {
                        node.nodeType = MapNodeType.MinorEnemy;
                    }
                    else if (y == config.preBossRestSiteLayer)
                    {
                        node.nodeType = MapNodeType.RestSite;
                    }
                    else if (y == config.numOfLayers - 1)
                    {
                        node.nodeType = MapNodeType.Boss;
                    }
                    else
                    {
                        node.nodeType = GetRandomNodeType(config, y);
                        
                        if (node.nodeType == MapNodeType.EliteEnemy && y < config.minEliteLayer)
                        {
                            node.nodeType = MapNodeType.MinorEnemy;
                        }
                        
                        EnsureNoConsecutiveTypes(grid, node);
                    }
                }
            }
        }

        private static void EnsureNoConsecutiveTypes(List<List<MapNode>> grid, MapNode node)
        {
            if (node.nodeType == MapNodeType.EliteEnemy || node.nodeType == MapNodeType.RestSite || node.nodeType == MapNodeType.Store)
            {
                foreach (var incomingPoint in node.incoming)
                {
                    if (grid[incomingPoint.y][incomingPoint.x].nodeType == node.nodeType)
                    {
                        node.nodeType = MapNodeType.MinorEnemy;
                        break;
                    }
                }
            }
        }

        private static MapNodeType GetRandomNodeType(MapConfigSO config, int currentLayer)
        {
            float totalWeight = config.defaultBlueprints.Sum(b => b.weight);
            float randomVal = Random.Range(0, totalWeight);
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

        private static List<int> GetRandomIndices(int count, int returnCount)
        {
            List<int> indices = new List<int>();
            for (int i = 0; i < count; i++) indices.Add(i);
            
            for (int i = 0; i < indices.Count; i++)
            {
                int temp = indices[i];
                int randomIndex = Random.Range(i, indices.Count);
                indices[i] = indices[randomIndex];
                indices[randomIndex] = temp;
            }

            return indices.Take(returnCount).ToList();
        }
    }
}
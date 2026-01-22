using Code.Expedition;
using UnityEngine;

namespace Code.Expedition.Logic
{
    public interface INodeLogic
    {
        void Execute(RuntimeExpeditionNode node);
    }
}
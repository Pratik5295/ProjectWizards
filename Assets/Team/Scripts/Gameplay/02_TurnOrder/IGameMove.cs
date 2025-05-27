
using System.Threading.Tasks;

namespace Team.Gameplay.TurnSystem
{
    public interface IGameMove
    {
        //Action to be performed
        Task PerformAsync();
    }
}


using System.Threading.Tasks;

namespace Team.Gameplay.TurnSystem
{
    public interface IGameMove
    {
        //Action the move will perform
        Task PerformAsync();
    }
}

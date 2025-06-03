using Team.Data;
using UnityEngine;

namespace Team.Gameplay.Characters
{
    [RequireComponent(typeof(Base_Ch))]
    public class CharacterReskinner : MonoBehaviour
    {
        [SerializeField]
        private Renderer[] _bodyMeshes;    //Convert it to a list if the model is going to have multiple meshes to color

        public void SetCharacterReskin(CharacterReskinData _reskinData)
        {
            for (int i = 0; i < _bodyMeshes.Length; i++)
            {
                _bodyMeshes[i].material = _reskinData.SkinMaterial;
            }
        }
    }
}

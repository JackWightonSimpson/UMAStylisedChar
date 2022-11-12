using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simpson.Character.StateMachine
{
    public class SetVariables : StateMachineBehaviour
    {
        [SerializeField]
        private List<AnimVar> enterVars = new List<AnimVar>();
        [SerializeField]
        private List<AnimVar> exitVars = new List<AnimVar>();
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            foreach (var animVar in enterVars)
            {
                switch (animVar.type)
                {
                    case VarType.Bool:
                        animator.SetBool(animVar.name, animVar.boolVal);
                        break;
                    case VarType.Int:
                        animator.SetInteger(animVar.name, animVar.intVal);
                        break;
                    case VarType.Float:
                        animator.SetFloat(animVar.name, animVar.floatVal);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            foreach (var animVar in exitVars)
            {
                switch (animVar.type)
                {
                    case VarType.Bool:
                        animator.SetBool(animVar.name, animVar.boolVal);
                        break;
                    case VarType.Int:
                        animator.SetInteger(animVar.name, animVar.intVal);
                        break;
                    case VarType.Float:
                        animator.SetFloat(animVar.name, animVar.floatVal);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }

        
        

    }

    [Serializable]
    public class AnimVar
    {
        public string name;
        public VarType type;
        public int intVal;
        public float floatVal;
        public bool boolVal;
    }

    [Serializable]
    public enum VarType
    {
        Bool,
        Int,
        Float
    }
}
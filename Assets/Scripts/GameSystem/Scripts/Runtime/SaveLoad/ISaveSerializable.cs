namespace GameSystem.SaveLoad
{
    public interface ISaveSerializable
    {

        public void SetSaveState(ObjectSaveData data);


        public void LoadSaveState(ObjectSaveData data);
    }
}
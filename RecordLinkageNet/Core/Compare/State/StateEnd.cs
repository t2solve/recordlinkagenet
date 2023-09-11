namespace RecordLinkageNet.Core.Compare.State
{
    public class StateEnd : CompareState
    {
        private object thinWeStore = null;

        public StateEnd() : base()
        {
            this.Name = "End";
            this.type = Type.End;
        }

        public override bool Load()
        {
            //return LoadDefaultDataMemeber(out thinWeStore);
            return true;
        }

        public override bool Save()
        {
            //return SaveDefaultDataMemeber(thinWeStore);
            return true;
        }
    }

}

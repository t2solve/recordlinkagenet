namespace RecordLinkageNet.Core.Compare.State
{

    public class StateProcess : CompareState
    {
        private MatchCandidateList listOfMachtes = null;

        public StateProcess() : base()
        {
            this.Name = "Process";
            this.type = Type.Process;
        }

        public MatchCandidateList ListOfMachtes { get => listOfMachtes; set => listOfMachtes = value; }

        public override bool Load()
        {
            return LoadDefaultDataMemeber(out listOfMachtes);
        }

        public override bool Save()
        {
            return SaveDefaultDataMemeber(listOfMachtes); ;
        }
    }
}

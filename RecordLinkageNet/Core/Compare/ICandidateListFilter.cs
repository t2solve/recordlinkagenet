namespace RecordLinkageNet.Core.Compare
{
    public interface ICandidateListFilter
    {
        ICandidateSet Apply(ICandidateSet g);
    }
}

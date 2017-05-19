namespace Abp.Application.Services
{
    public interface INode<T>
    {
        int? Id { get; set; }
        int? ParentId { get; set; }
        string Name { get; set; }
        bool? Disabled { get; set; }
        T CustomData { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
namespace Abp.Application.Services.Dto
{
    public class PagedInputDto : IPagedResultRequest
    {
        [Range(1, AbpExConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public PagedInputDto()
        {
            MaxResultCount = AbpExConsts.DefaultPageSize;
        }
    }
}

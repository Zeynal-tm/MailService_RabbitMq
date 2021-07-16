using AutoMapper;
using EmailGetService.Models;
using EmailGetService.Models.DTO;

namespace EmailGetService.Mapping
{
    public class EmailMappingProfile : Profile
    {
        public EmailMappingProfile()
        {
            CreateMap<Email, EmailDto>();
            CreateMap<Attachment, AttachmentDto>();
        }
    }
}

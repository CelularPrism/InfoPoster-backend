using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Selectel;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class FileRepository
    {
        private readonly OrganizationContext _context;

        public FileRepository(OrganizationContext context)
        {
            _context = context;
        }

        public async Task<List<SelectelFileURLModel>> GetSelectelFiles(Guid applicationId, int place) =>
            await _context.FileToApplication.Where(f => f.ApplicationId == applicationId && f.Place == place)
                                            .Join(_context.SelectelFileURLs,
                                                       f => f.FileId,
                                                       s => s.Id,
                                                       (f, s) => s)
                                            .ToListAsync();

        public async Task<FileToApplication> GetPrimaryFile(Guid applicationId, int place) =>
            await _context.FileToApplication.Where(f => f.ApplicationId == applicationId && f.Place == place && f.IsPrimary == true).FirstOrDefaultAsync();

        public async Task RemoveFile(Guid fileId, Guid applicationId)
        {
            var file = await _context.FileToApplication.FirstOrDefaultAsync(f => f.FileId == fileId && f.ApplicationId == applicationId);
            if (file != null)
            {
                _context.FileToApplication.Remove(file);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddFileToApplication(FileToApplication file)
        {
            await _context.FileToApplication.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task AddSelectelFile(SelectelFileURLModel file)
        {
            await _context.SelectelFileURLs.AddAsync(file);
            await _context.SaveChangesAsync();
        }
    }
}

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

        public async Task<FileToApplication> GetApplicationFile(Guid id) =>
            await _context.FileToApplication.Where(f => f.FileId == id).FirstOrDefaultAsync();

        public async Task<FileToApplication> GetApplicationFileByApplication(Guid applicationId) =>
            await _context.FileToApplication.Where(f => f.FileId == applicationId).FirstOrDefaultAsync();

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
            if (file.IsPrimary == true)
            {
                var listPrimary = await _context.FileToApplication.Where(f => f.ApplicationId == file.ApplicationId && f.IsPrimary).ToListAsync();
                if (listPrimary.Count > 0)
                {
                    foreach (var item in listPrimary)
                    {
                        item.IsPrimary = false;
                    }
                    _context.FileToApplication.UpdateRange(listPrimary);
                }
            }

            await _context.FileToApplication.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFileToApplication(FileToApplication file)
        {
            if (file.IsPrimary == true)
            {
                var listPrimary = await _context.FileToApplication.Where(f => f.ApplicationId == file.ApplicationId && f.IsPrimary).ToListAsync();
                if (listPrimary.Count > 0)
                {
                    foreach (var item in listPrimary)
                    {
                        item.IsPrimary = false;
                    }
                    _context.FileToApplication.UpdateRange(listPrimary);
                }
            }

            _context.FileToApplication.Update(file);
            await _context.SaveChangesAsync();
        }

        public async Task AddSelectelFile(SelectelFileURLModel file)
        {
            await _context.SelectelFileURLs.AddAsync(file);
            await _context.SaveChangesAsync();
        }
    }
}

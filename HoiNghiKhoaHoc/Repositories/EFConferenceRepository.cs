﻿using HoiNghiKhoaHoc.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HoiNghiKhoaHoc.Repositories
{
    public class EFConferenceRepository : IConferenceRepository
    {
        private readonly ApplicationDbContext _context;

        public EFConferenceRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddConferenceAsync(Conference conference)
        {
            await _context.Conferences.AddAsync(conference);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteConferenceAsync(int id)
        {
            var conference = await _context.Conferences.FindAsync(id);
            if (conference == null)
            {
                throw new Exception("Conference not found.");
            }
            _context.Conferences.Remove(conference);    
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Conference>> GetAllConferencesAsync()
        {
            return await _context.Conferences
                .Include(c => c.Category) // Load thông tin Category
                .ToListAsync();
        }

        public async Task<IEnumerable<Conference>> GetAllConferencesGlobalAsync()
        {
            return await _context.Conferences
                .Where(c => c.IsInternational == true)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conference>> GetAllConferencesPastAsync()
		{
			return await _context.Conferences
				.Where(c => c.EndDate < DateTime.Now)
				.OrderByDescending(c => c.EndDate)
				.Include(c => c.Category)
				.ToListAsync();
		}

		public async Task<IEnumerable<Conference>> GetAllConferencesUpcomingAsync()
		{
			return await _context.Conferences
				.Where(c => c.StartDate > DateTime.Now)
				.OrderBy(c => c.StartDate)
				.ToListAsync();
		}

		public async Task<Conference> GetConferenceByIdAsync(int id)
		{
			return await _context.Conferences
				.Include(c => c.Category)
				.FirstOrDefaultAsync(c => c.Id == id);
		}


		public async Task<IEnumerable<Conference>> GetConferenceByIdCategoryAsync(Conference conference)
		{
			return await _context.Conferences
		.Where(c => c.Id != conference.Id && c.CategoryId == conference.CategoryId)
		.OrderByDescending(c => c.StartDate)
		.Take(4)
		.ToListAsync();
		}


		public async Task<Conference?> GetPastConferenceDetailsByIdAsync(int id)
		{
			return await _context.Conferences
				.Include(c => c.Category)
				//.Include(c => c.Country)
				.FirstOrDefaultAsync(c => c.Id == id && c.EndDate < DateTime.Now);
		}

		public async Task UpdateConferenceAsync(Conference conference)
        {
            var existingConference = await _context.Conferences.FindAsync(conference.Id);
            if (existingConference == null)
            {
                throw new Exception("Conference not found.");
            }
            _context.Entry(existingConference).CurrentValues.SetValues(conference);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Conference>> GetUpcomingConferencesAsync()
        {
            var now = DateTime.Now;
            return await _context.Conferences
                .Where(c => c.StartDate > now)
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conference>> GetPastConferencesAsync()
        {
            var now = DateTime.Now;
            return await _context.Conferences
                .Where(c => c.EndDate < now)
                .Include(c => c.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<Conference>> SearchConferencesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllConferencesAsync();

            string normalized = RemoveDiacritics(searchTerm.ToLower());

            var all = await _context.Conferences.ToListAsync();

            return all.Where(c =>
                RemoveDiacritics(c.Title).ToLower().Contains(normalized) ||
                RemoveDiacritics(c.Description ?? "").ToLower().Contains(normalized) ||
                RemoveDiacritics(c.Location ?? "").ToLower().Contains(normalized) ||
                RemoveDiacritics(c.Organizer ?? "").ToLower().Contains(normalized));
        }

    
        private string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";

            var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
            var sb = new System.Text.StringBuilder();

            foreach (var ch in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        //Lay nhung hoi nghi dang hoat dong
		public async Task<IEnumerable<Conference>> GetActiveConferencesAsync()
		{
			return await _context.Conferences.Where(c => c.IsActive).ToListAsync();
		}
        public async Task AddConferenceSpeakersAsync(List<ConferenceSpeaker> conferenceSpeakers)
        {
            if (conferenceSpeakers == null || conferenceSpeakers.Count == 0)
                return;

            await _context.ConferenceSpeakers.AddRangeAsync(conferenceSpeakers);
            await _context.SaveChangesAsync();
        }

    }
}
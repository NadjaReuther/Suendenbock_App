using Microsoft.AspNetCore.Mvc;
using Suendenbock_App.Data;
using Suendenbock_App.Models.Domain;
using Suendenbock_App.Services;

namespace Suendenbock_App.Controllers
{
    public class GuildController : BaseOrganizationController
    {
        private readonly ICachedDataService _cachedData;
        public GuildController(ApplicationDbContext context, IImageUploadService imageUploadService, ICachedDataService cachedData) : base(context, imageUploadService)
        {
            _cachedData = cachedData;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Form(int id = 0)
        {
            // Load common ViewBag data
            LoadCommonViewBagData();
            //specific ViewBag data for Guild
            ViewBag.LightCards = await _cachedData.GetLightCardsAsync();
            ViewBag.Abenteuerrang = await _cachedData.GetAbenteuerrangeAsync();
            ViewBag.Anmeldungsstatus =await _cachedData.GetAnmeldungsstatusseAsync();

            if (id > 0)
            {
                var guild = _context.Guilds.Find(id);
                if (guild == null)
                {
                    return NotFound();
                }
                return View(guild);
            }
            return View(new Guild());
        }

        [HttpPost]
        public async Task<IActionResult> CreateEdit(Guild guild, IFormFile? gildenlogo)
        {
            try
            {
                var uploadedImagePath = await ProcessImageUpload(gildenlogo, guild.Name, "guild");
                if (uploadedImagePath != null)
                {
                    guild.ImagePath = uploadedImagePath;
                }

                if (guild.Id == 0)
                {
                    _context.Guilds.Add(guild);
                }
                else
                {
                    var guildToUpdate = _context.Guilds.Find(guild.Id);
                    if (guildToUpdate == null)
                    {
                        return NotFound();
                    }
                    UpdateGuildProperties(guildToUpdate, guild);

                    if (!string.IsNullOrEmpty(guild.ImagePath))
                    {
                        DeleteOldImage(guildToUpdate.ImagePath);
                        guildToUpdate.ImagePath = guild.ImagePath;
                    }
                }
                _context.SaveChanges();
                SetMessage(true, "Gilde");
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fehler beim Speichern: {ex.Message}";
                return RedirectToAction("Form", new { id = guild.Id });
            }
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var guild = _context.Guilds.Find(id);
            if (guild == null)
            {
                return NotFound();
            }
            
            DeleteOldImage(guild.ImagePath);
            
            _context.Guilds.Remove(guild);
            _context.SaveChanges();

            SetMessage(true, "Gilde gelöscht");
            return RedirectToAction("Index","Admin");
        }
        public void UpdateGuildProperties(Guild guildToUpdate, Guild guild)
        {
            guildToUpdate.Name = guild.Name;
            guildToUpdate.Description = guild.Description;
            guildToUpdate.LightCardId = guild.LightCardId;
            guildToUpdate.AbenteuerrangId = guild.AbenteuerrangId;
            guildToUpdate.AnmeldungsstatusId = guild.AnmeldungsstatusId;
            guildToUpdate.leader = guild.leader;
            guildToUpdate.vertreter = guild.vertreter;
        }
    }
}

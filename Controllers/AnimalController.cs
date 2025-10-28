using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AngularSPAWebAPI.Controllers
{
    /// <summary>
    /// Resources Web API controller.
    /// </summary>
    [Route("api/[controller]")]
    // Authorization policy for this API.
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class AnimalController : Controller
    {
        private readonly AnimalService _animalService;

        private readonly UserManager<ApplicationUser> _manager;

        public AnimalController(UserManager<ApplicationUser> manager)
        {
            _animalService = new AnimalService();
            _manager = manager;
        }

        [HttpGet("GetAnimalInfoByExpID")]
        public async Task<IActionResult> GetAnimalInfoByExpID(int expId)
        {
            var res = await _animalService.GetAnimalByExpIDAsync(expId);
            return new JsonResult(res);
        }

        [HttpPost("CreateAnimal")]
        public async Task<IActionResult> CreateAnimal([FromBody]Animal animal)
        {

            // throw new Exception("This Experiment Name was already taken!");
            if (await _animalService.DoesAnimalIDExistAsync(animal.UserAnimalID, animal.ExpID))
            {
                return new JsonResult("Taken");
            }
            else
            {
                return new JsonResult(await _animalService.InsertAnimalAsync(animal));
            }
        }

        [HttpPost("UpdateAnimal")]
        public async Task<IActionResult> UpdateAnimal([FromBody]Animal animal)
        {

            await _animalService.UpdateAnimalAsync(animal);
            return new JsonResult("Done!");

        }

        [HttpDelete("DeleteAnimalById")]
        public async Task<IActionResult> DeleteAnimalById(int animalID)
        {
            await _animalService.DeleteAnimalByAnimalIDAsync(animalID);
            return new JsonResult("Done!");

        }

        [HttpGet("GetAllStrain")]
        public async Task<IActionResult> GetAllStrain()
        {
            var res = await _animalService.GetStrainListAsync();
            return new JsonResult(res);
        }

        [HttpGet("GetAllGenoByStrainID")]
        public async Task<IActionResult> GetAllGenoByStrainID(int ID)
        {
            var res = await _animalService.GetGenoListAsync(ID);
            return new JsonResult(res);
        }

        [HttpGet("GetCountOfAnimals")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCountOfAnimals()
        {
            var res = await _animalService.GetCountOfAnimalsAsync();
            return new JsonResult(res);
        }

        [HttpGet("UserAnimalIDExist")]
        public async Task<IActionResult> UserAnimalIDExist(string UserAnimalID, int ExpID)
        {

            bool flag = await _animalService.IsUserAnimalIDExistAsync(UserAnimalID, ExpID);
            if (flag == true)
            {
                return new JsonResult("Exist");
            }
            else
            {
                return new JsonResult("Not Exist");
            }

        }


        [HttpGet("EditUserAnimalID")]
        public async Task<IActionResult> EditUserAnimalID(string EditedUserAnimalId, int OldAnimalId, int ExpId)
        {
            (int ExistingAnimalIdToUse, bool isAnimalInfocompleted) = await _animalService.GetAnimalIDByUserAnimalIdAndExpIdAsync(EditedUserAnimalId, ExpId);

            bool updated = await _animalService.ReplaceAnimalIdAsync(OldAnimalId, ExistingAnimalIdToUse);
            if (isAnimalInfocompleted)
            {
                var user = await _manager.GetUserAsync(HttpContext.User);
                var userID = user.Id;
                UploadService uploadService = new UploadService();
                await uploadService.SetAsResolvedForEditedAnimalIdAsync(ExistingAnimalIdToUse, userID);
            }
            if (updated == true)
            {
                return new JsonResult("Successful");
            }
            else
            {
                return new JsonResult("Failed");
            }
        }

    }
}

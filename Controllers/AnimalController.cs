using AngularSPAWebAPI.Models;
using AngularSPAWebAPI.Services;
using IdentityServer4.AccessTokenValidation;
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
    [Authorize(AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme, Policy = "Access Resources")]
    public class AnimalController : Controller
    {
        private readonly AnimalService _animalService;

        private readonly UserManager<ApplicationUser> _manager;

        public AnimalController(UserManager<ApplicationUser> manager)
        {
            _animalService = new AnimalService();
            _manager = manager;
        }

        private async Task<ApplicationUser> GetCurrentUser()
        {
            return await _manager.GetUserAsync(HttpContext.User);
        }

        [HttpGet("GetAnimalInfoByExpID")]
        public IActionResult GetAnimalInfoByExpID(int expId)
        {
            var user = GetCurrentUser();
            var userID = user.Result.Id;

            return new JsonResult(_animalService.GetAnimalByExpID(expId));
        }

        [HttpPost("CreateAnimal")]
        public IActionResult CreateAnimal([FromBody]Animal animal)
        {

            // throw new Exception("This Experiment Name was already taken!");
            if (_animalService.DoesAnimalIDExist(animal.UserAnimalID, animal.ExpID))
            {
                return new JsonResult("Taken");
            }
            else
            {
                return new JsonResult(_animalService.InsertAnimal(animal));
            }
        }

        [HttpPost("UpdateAnimal")]
        public IActionResult UpdateAnimal([FromBody]Animal animal)
        {

            _animalService.UpdateAnimal(animal);
            return new JsonResult("Done!");

        }

        [HttpDelete("DeleteAnimalById")]
        public IActionResult DeleteAnimalById(int animalID)
        {
            _animalService.DeleteAnimalByAnimalID(animalID);
            return new JsonResult("Done!");

        }

        [HttpGet("GetAllStrain")]
        public IActionResult GetAllStrain()
        {

            return new JsonResult(_animalService.GetStrainList());
        }

        [HttpGet("GetAllGenoByStrainID")]
        public IActionResult GetAllGenoByStrainID(int ID)
        {

            return new JsonResult(_animalService.GetGenoList(ID));
        }

        [HttpGet("GetCountOfAnimals")]
        [AllowAnonymous]
        public IActionResult GetCountOfAnimals()
        {
            return new JsonResult(_animalService.GetCountOfAnimals());
        }

        [HttpGet("UserAnimalIDExist")]
        public IActionResult UserAnimalIDExist(string UserAnimalID, int ExpID)
        {

            bool flag = _animalService.IsUserAnimalIDExist(UserAnimalID, ExpID);
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
        public IActionResult EditUserAnimalID(string EditedUserAnimalId, int OldAnimalId, int ExpId)
        {
            (int ExistingAnimalIdToUse, bool isAnimalInfocompleted) = _animalService.GetAnimalIDByUserAnimalIdAndExpId(EditedUserAnimalId, ExpId);

            bool updated = _animalService.ReplaceAnimalId(OldAnimalId, ExistingAnimalIdToUse);

            if (isAnimalInfocompleted)
            {
                var user = GetCurrentUser();
                var userID = user.Result.Id;

                UploadService uploadService = new UploadService();
                uploadService.SetAsResolvedForEditedAnimalId(ExistingAnimalIdToUse, userID);

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

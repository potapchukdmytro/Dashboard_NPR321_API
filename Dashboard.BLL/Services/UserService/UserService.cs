using AutoMapper;
using Dashboard.BLL.Services.ImageService;
using Dashboard.DAL;
using Dashboard.DAL.Models.Identity;
using Dashboard.DAL.Repositories.UserRepository;
using Dashboard.DAL.ViewModels;

namespace Dashboard.BLL.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IImageService _imageService;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper, IImageService imageService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _imageService = imageService;
        }

        public async Task<ServiceResponse> AddImageFromUserAsync(UserImageVM model)
        {
            var user = await _userRepository.GetByIdAsync(model.UserId);

            if(user == null)
            {
                return ServiceResponse.BadRequestResponse($"Користувача з id {model.UserId} не знайдено"); 
            }

            var response = await _imageService.SaveImageFromFileAsync(Settings.UserImagesPath, model.Image);

            if(!response.Success)
            {
                return response;
            }

            user.Image = response.Payload.ToString();
            var result = await _userRepository.UpdateAsync(user);

            if(!result.Succeeded)
            {
                return ServiceResponse.BadRequestResponse(result.Errors.First().Description);
            }

            return ServiceResponse.OkResponse("Зображення успішно додано");
        }

        public async Task<ServiceResponse> CreateAsync(CreateUpdateUserVM model)
        {
            if (!await _userRepository.IsUniqueUserNameAsync(model.UserName))
            {
                return ServiceResponse.BadRequestResponse($"{model.UserName} вже викорстовується");
            }

            if (!await _userRepository.IsUniqueEmailAsync(model.Email))
            {
                return ServiceResponse.BadRequestResponse($"{model.Email} вже викорстовується");
            }

            var user = _mapper.Map<User>(model);
            user.Id = Guid.NewGuid().ToString();

            var result = await _userRepository.CreateAsync(user, model.Password);
            
            if (!result.Succeeded)
            {
                return ServiceResponse.BadRequestResponse(result.Errors.First().Description);
            }

            result = await _userRepository.AddToRoleAsync(user, model.Role);

            return ServiceResponse.ByIdentityResult(result, "Користувач успішно створений");
        }

        public async Task<ServiceResponse> DeleteAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return ServiceResponse.BadRequestResponse($"Користувача з id {id} не знайдено");
            }

            var result = await _userRepository.DeleteAsync(user);

            return ServiceResponse.ByIdentityResult(result, $"Користувача з id {id} успішно видалено");
        }

        public async Task<ServiceResponse> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();

            var models = _mapper.Map<List<UserVM>>(users);

            return ServiceResponse.OkResponse("Користувачів отримано", models);
        }

        public async Task<ServiceResponse> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email, true);

            if (user == null)
            {
                return ServiceResponse.BadRequestResponse($"Користувача з поштою {email} не знайдено");
            }

            var model = _mapper.Map<UserVM>(user);

            return ServiceResponse.OkResponse("Користувача знайдено", model);
        }

        public async Task<ServiceResponse> GetByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id, true);

            if (user == null)
            {
                return ServiceResponse.BadRequestResponse($"Користувача з id {id} не знайдено");
            }

            var model = _mapper.Map<UserVM>(user);

            return ServiceResponse.OkResponse("Користувача знайдено", model);
        }

        public async Task<ServiceResponse> GetByUserNameAsync(string userName)
        {
            var user = await _userRepository.GetByUsernameAsync(userName, true);

            if (user == null)
            {
                return ServiceResponse.BadRequestResponse($"Користувача з іменем {userName} не знайдено");
            }

            var model = _mapper.Map<UserVM>(user);

            return ServiceResponse.OkResponse("Користувача знайдено", model);
        }

        public async Task<ServiceResponse> UpdateAsync(CreateUpdateUserVM model)
        {
            if(string.IsNullOrEmpty(model.Id))
            {
                return ServiceResponse.BadRequestResponse("Не вдалося ідентифікувати користувача");
            }

            var user = await _userRepository.GetByIdAsync(model.Id, true);

            if(user == null)
            {
                return ServiceResponse.BadRequestResponse("Користувача не знайдено");
            }

            if(model.Email != user.Email)
            {
                if (!await _userRepository.IsUniqueEmailAsync(model.Email))
                {
                    return ServiceResponse.BadRequestResponse($"Пошта {model.Email} вже використовується");
                }
            }

            if (model.UserName != user.UserName)
            {
                if (!await _userRepository.IsUniqueUserNameAsync(model.UserName))
                {
                    return ServiceResponse.BadRequestResponse($"Ім'я {model.UserName} вже використовується");
                }
            }

            user = _mapper.Map(model, user);

            var result = await _userRepository.UpdateAsync(user);

            if (user.UserRoles.First().Role.NormalizedName != model.Role.ToUpper())
            {
                // Видалити попередню роль та записати нову
            }

            return ServiceResponse.ByIdentityResult(result, "Користувач успішно оновлений");
        }
    }
}

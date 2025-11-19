using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.DTOs.UserDto
{
    /// <summary>
    /// DTO para atualização de usuários.
    /// </summary>
    public class UpdateUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserEnum.State State { get; set; }

        /// <summary>
        /// Converte o DTO para a entidade de domínio User.
        /// </summary>
        public User ToDomainUser()
        {
            return new User(
                name: this.Name,
                email: this.Email,
                password: this.Password,
                state: this.State
            );
        }
    }
}

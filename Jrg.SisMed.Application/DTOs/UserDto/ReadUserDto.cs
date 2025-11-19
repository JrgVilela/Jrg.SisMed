using Jrg.SisMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.DTOs.UserDto
{
    /// <summary>
    /// DTO para leitura de usuários.
    /// </summary>
    public class ReadUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserEnum.State State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Cria um ReadUserDto a partir de uma entidade User do domínio.
        /// </summary>
        public static ReadUserDto FromDomainUser(User user)
        {
            return new ReadUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                State = user.State,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}

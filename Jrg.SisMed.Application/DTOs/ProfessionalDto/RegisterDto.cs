using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.DTOs.ProfessionalDto
{
    public class RegisterDto
    {
        //Dados do Profissional
        public string Name { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string RegisterNumber { get; set; } = string.Empty;
        public ProfessionalType ProfessionalType { get; set; }

        //Dados do Usuário
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        //Dados de Contato
        public string Phone { get; set; } = string.Empty;

        //Dados de Endereço
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string? Complement { get; set; }
        public string Neighborhood { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;

        //Dados da Organização (Consultorio ou Empresa)
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;

        public Professional ToDomain()
        {
            switch (ProfessionalType)
            {
                case ProfessionalType.Psychologist:
                    return ToDomainPsychology();
                //case ProfessionalType.Nutritionist:
                //    break;
                default:
                    return null;
            }
        }

        private Psychologist ToDomainPsychology()
        {
            Psychologist psychologist = new Psychologist(
                name: Name,
                cpf: Cpf,
                rg: null,
                birthDate: null,
                crp: RegisterNumber,
                gender: ProfessionalEnum.Gender.Other
            );

            psychologist.AddPhone(new ProfessionalPhone(psychologist, new Phone(
                ddi: Phone.Split(' ')[0],
                ddd: Phone.Split(' ')[1],
                number: Phone.Split(' ')[2]
            )));

            psychologist.AddUser(new User(Name, Email, Password, UserEnum.State.Active));

            psychologist.AddAddress(new ProfessionalAddress(psychologist, new Address(
                street: Street,
                number: Number,
                complement: Complement,
                neighborhood: Neighborhood,
                zipCode: ZipCode,
                city: City,
                state: State
            )));

            psychologist.AddOrganization(new OrganizationProfessional(
                organization: new Organization(
                    nameFantasia: NomeFantasia,
                    razaoSocial: RazaoSocial,
                    cnpj: Cnpj
                ),
                professional: psychologist
            ));

            return psychologist;
        }
    }
}

using Jrg.SisMed.Domain.Interfaces;
using Jrg.SisMed.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Infra.Data.UnitOfWork
{
    /// <summary>
    /// Implementação do padrão Unit of Work para gerenciar transações.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor do Unit of Work.
        /// </summary>
        /// <param name="context">Contexto do banco de dados.</param>
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Salva todas as alterações pendentes no banco de dados.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento.</param>
        /// <returns>Número de registros afetados.</returns>
        /// <exception cref="DbUpdateException">Lançado quando há erro ao salvar no banco.</exception>
        /// <exception cref="DbUpdateConcurrencyException">Lançado quando há conflito de concorrência.</exception>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                // Log do erro (adicionar logging futuramente)
                throw new InvalidOperationException("Erro ao salvar alterações no banco de dados.", ex);
            }
        }

        /// <summary>
        /// Libera os recursos utilizados pelo contexto.
        /// </summary>
        public void Dispose()
        {
            _context?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera os recursos utilizados pelo contexto de forma assíncrona.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
            }
            GC.SuppressFinalize(this);
        }
    }
}

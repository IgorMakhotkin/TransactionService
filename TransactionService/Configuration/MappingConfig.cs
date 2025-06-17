using Mapster;
using TransactionService.Models;
using TransactionService.Models.Entity;
using TransactionService.Models.Models.Response;
using TransactionService.Models.Response;

namespace TransactionService.Configuration
{
    public class MappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {

            config.NewConfig<TransactionRecord, CreditResponse>()
                .Map(dest => dest.ClientBalance, src => src.Amount)
                .Map(dest => dest.InsertDateTime, src => src.CreatedAt);

            config.NewConfig<Balance, BalanceResponse>()
                .Map(dest => dest.ClientBalance, src => src.Amount)
                .Map(dest => dest.BalanceDateTime, src => DateTime.UtcNow);

            config.NewConfig<CreditRequest, CreditTransaction>()
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.ClientId, src => src.ClientId)
                .Map(dest => dest.DateTime, src => src.DateTime)
                .Map(dest => dest.Id, src => src.Id);

            config.NewConfig<DebitRequest, DebitTransaction>()
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.ClientId, src => src.ClientId)
                .Map(dest => dest.DateTime, src => src.DateTime)
                .Map(dest => dest.Id, src => src.Id);

            config.NewConfig<Balance, BalanceDtoResponse>()
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.ClientId, src => src.ClientId);
        }
    }
}

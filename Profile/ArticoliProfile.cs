using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Dtos;
using ArticoliWebService.Models;
using AutoMapper;

namespace ArticoliWebService.Profiles
{
    public class ArticoliProfile: Profile
    {
        public ArticoliProfile()
        {
            // Ora che IvaDto ha un costruttore senza parametri, questa mappatura è valida.
            // AutoMapper creerà un IvaDto vuoto e poi popolerà le proprietà.
            CreateMap<Iva, IvaDto>()
                .ForMember(dest => dest.Descrizione,
                           opt => opt.MapFrom(src => Clean(src.Descrizione)));
            
             CreateMap<BarcodeEan, BarcodeEanDto>()
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.BarCode))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.IdTipoArt));
                       
            CreateMap<FamAssort, FamAssortDto>();

            CreateMap<Ingredienti, IngredientiDto>();

            CreateMap<Articoli, ArticoliDto>()
                .ForMember(dest => dest.Descrizione, opt => opt.MapFrom(src => Clean(src.Descrizione)))
                .ForMember(dest => dest.Um, opt => opt.MapFrom(src => Clean(src.Um)))
                .ForMember(dest => dest.CodStat, opt => opt.MapFrom(src => Clean(src.CodStat)))
                .ForMember(dest => dest.IdStatoArticolo, opt => opt.MapFrom(src => Clean(src.IdStatoArt)))
                .ForMember(dest => dest.PzCart, opt => opt.MapFrom(src => Clean(src.PzCart)))            
   /*             .ForMember(
                    dest => dest.Categoria,
                    opt => opt.MapFrom(src => $"{Clean(src.IdFamAss)} {(src.FamAssort != null ? Clean(src.FamAssort.Descrizione) : string.Empty)}")
                )*/
                .ForMember(dest => dest.Categoria, opt => opt.MapFrom(src => src.FamAssort != null ? src.FamAssort.Descrizione : string.Empty))                
                .ForMember(dest => dest.Ean, opt => opt.MapFrom(src => src.Barcode));
        }

        private static string Clean(string? value)
            => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        private static int Clean(int? value)
            => (value == null || value == 0) ? 0 : value.Value;
            
    }
}
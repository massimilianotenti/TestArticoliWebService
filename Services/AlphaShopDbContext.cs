using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArticoliWebService.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticoliWebService.Services
{
    public class AlphaShopDbContext : DbContext
    {
        // Costruttore ler l'iniezione delle dipendenze
        public AlphaShopDbContext(DbContextOptions<AlphaShopDbContext> options) : base(options)
        {

        }

        // Proprietà DbSet, ognuna rappresenta una tabella del database
        public virtual DbSet<Articoli> Articoli { get; set; }
        public virtual DbSet<BarcodeEan> BarcodeEans { get; set; }
        public virtual DbSet<FamAssort> FamAssorts { get; set; }
        public virtual DbSet<Ingredienti> Ingredienti { get; set; }
        public virtual DbSet<Iva> Iva { get; set; }

        // Le Data Annotations sui model hanno lo scopo principale di validazione del modello, i 
        // dati devono essere validi prima di arrivare al database
        // Nel DbContext utilizziamo le Fluent API per configura tutti gli elementi delle nostre classi 
        // modello, hanno una priorità maggiore rispetto a quanto definito nei models con i Data Annotations.
        // Servono per il mapping del database. Se il campo della chiave primaria del modello Articoli si fosse 
        // chiamato ArticoliId o semplicemente Id non ci sarebbe stato bisogno
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articoli>()
                .HasKey(a => new { a.CodArt });
            // Articoli ↔ Ingredienti (Uno-a-Uno)
            // Un Articolo ha uno e solo un Ingrediente, e viceversa. La chiave esterna (CodArt) risiede nella tabella Ingredienti
            modelBuilder.Entity<Articoli>()
                .HasOne<Ingredienti>(s => s.Ingrediente)    
                .WithOne(g => g.Articolo)                   
                .HasForeignKey<Ingredienti>(a => a.CodArt);
            // Articoli ↔ Iva (Molti-a-Uno)
            // Ad una aliquota Iva corrispondono molti Articoli. La chiave esterna (IdIva) risiede nella tabella Articoli.
            modelBuilder.Entity<Articoli>()
                .HasOne<Iva>(a => a.Iva)                    
                .WithMany(g => g.Articoli)                  
                .HasForeignKey(s => s.IdIva);
            // Articoli ↔ FamAssort (Molti-a-Uno)
            // Ad una Famiglia di Assortimento corrispondono molti Articoli. La chiave esterna (IdFamAss) risiede nella tabella Articoli
            modelBuilder.Entity<Articoli>()
                .HasOne<FamAssort>(a => a.FamAssort)        
                .WithMany(g => g.Articoli)                  
                .HasForeignKey(s => s.IdFamAss);           
            // BarcodeEan ↔ Articoli (Molti-a-Uno)
            // Ad un Articolo corrispondono molti Barcode. La chiave esterna (CodArt) risiede nella tabella BarcodeEan.
            modelBuilder.Entity<BarcodeEan>()
                .HasOne<Articoli>(a => a.Articolo)         
                .WithMany(g => g.Barcode)                  
                .HasForeignKey(s => s.CodArt);              
        }


    }
}
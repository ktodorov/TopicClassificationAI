using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using DataAccessLayer.Contexts;

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(TopicClassificationContext))]
    [Migration("20160529161423_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("DataAccessLayer.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .IsRequired();

                    b.HasKey("Id");
                });

            modelBuilder.Entity("DataAccessLayer.Models.ArticleTopic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleId");

                    b.Property<int>("Topic");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("DataAccessLayer.Models.Word", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Text")
                        .IsRequired();

                    b.HasKey("Id");
                });

            modelBuilder.Entity("DataAccessLayer.Models.WordOccurence", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ArticleId");

                    b.Property<int>("TimesOccured");

                    b.Property<int>("WordId");

                    b.HasKey("Id");
                });

            modelBuilder.Entity("DataAccessLayer.Models.ArticleTopic", b =>
                {
                    b.HasOne("DataAccessLayer.Models.Article")
                        .WithMany()
                        .HasForeignKey("ArticleId");
                });

            modelBuilder.Entity("DataAccessLayer.Models.WordOccurence", b =>
                {
                    b.HasOne("DataAccessLayer.Models.Article")
                        .WithMany()
                        .HasForeignKey("ArticleId");

                    b.HasOne("DataAccessLayer.Models.Word")
                        .WithMany()
                        .HasForeignKey("WordId");
                });
        }
    }
}

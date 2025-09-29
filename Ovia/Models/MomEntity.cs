using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;

namespace Ovia.Models
{
    public class MomEntity : DbContext
    {

        public MomEntity()
        {
            
        }

        public MomEntity(DbContextOptions options):base(options)
        {
            
        }

        public DbSet<CustomerInfo> CustomerInfo { get; set; }
        public DbSet<EventGuest> EventGuest { get; set; }
        public DbSet<PointsDelay> PointsDelay { get; set; }
        public DbSet<PointValue> PointValue { get; set; }
        public DbSet<EditProfileHistory> EditProfileHistory { get; set; }
        public DbSet<CustomerAttribute> CustomerAttributes { get; set; }
        public DbSet<CustomerAttachments> CustomerAttachments { get; set; }
        public DbSet<CustomerNetwork> CustomerNetwork { get; set; }
        public DbSet<CustomerRequestsData> CustomerRequestsData { get; set; }
        public DbSet<RefreshTokens> RefreshTokens { get; set; }
        public DbSet<Subscribe> Subscribe { get; set; }
        public DbSet<CoursSectionDetails> CoursSectionDetails { get; set; }
        public DbSet<HomePhotos> HomePhotos { get; set; }

        public DbSet<Teams> Teams { get; set; }
        public DbSet<Generation> Generation { get; set; }
        public DbSet<UserImg> UserImgs { get; set; }
        public DbSet<Rank> Rank { get; set; }
        public DbSet<RankHistory> RankHistory { get; set; }
        public DbSet<CustomerCustomerRoleMapping> CustomerCustomerRoleMapping { get; set; }
        public DbSet<TrainningMapping> TrainningMapping { get; set; }
        public DbSet<Trainning> Trainning { get; set; }

        public DbSet<City> City { get; set; }
        public DbSet<Countries> Country { get; set; }
        public DbSet<CoursePackageMapping> CoursePackageMappings { get; set; }
        public DbSet<Governorate> Governorate { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageImage> PackageImages { get; set; }
        public DbSet<PackagesTypes> PackagesTypes { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<CustomerPackageSelect> Customerselectpackages { get; set; }
        public DbSet<CustomerAccountBalanceSingUp> CustomerAccountBalanceSingUps { get; set; }
        public DbSet<CustomerAccountBalance> CustomerAccountBalances { get; set; }
        public DbSet<PayToken> PayTokens { get; set; }
        public DbSet<PointEquation> pointEquations { get; set; }
        public DbSet<NsStartingBalance> NsStartingBalances { get; set; }
        public DbSet<NsPaymentHistory> NsPaymentHistory { get; set; }
        public DbSet<CourseCustomerMapping> CourseCustomerMapping { get; set; }
        public DbSet<CourseDetails> CourseDetails { get; set; }
        public DbSet<CourseInstructorMapping> CourseInstructorMappings { get; set; }
         public DbSet<Instructor> Instructors { get; set; }
        public DbSet<CourseComment> CourseComment { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CustomerAnnualRenewal> CustomerAnnualRenewal { get; set; }
        public DbSet<CustomerPackageSelect> CustomerPackageSelect { get; set; }
        public DbSet<CustomerTransaction> CustomerTransaction { get; set; }

        public DbSet<Photo> Photo { get; set; }


        public DbSet<Log> log { get; set; }
        public DbSet<PointProcess> PointProcess { get; set; }
        public DbSet<Points> Points { get; set; }
        public DbSet<DistributorsAddonCoins> DistributorsAddonCoins { get; set; }
        public DbSet<DistributorsCommunityPersonal> DistributorsCommunityPersonal { get; set; }
        public DbSet<Distributors_Summit_Coins> Distributors_Summit_Coins { get; set; }
        public DbSet<ProcessType> ProcessType { get; set; }
        public DbSet<Profit> Profit { get; set; }
        public DbSet<CustomerDiscounts> CustomerDiscounts { get; set; }
        public DbSet<Content> Content { get; set; }
        public virtual DbSet<VisabltyCash> VisabltyCash { get; set; }
        public virtual DbSet<RequestCash> RequestCash { get; set; }
        public virtual DbSet<RequestType> RequestType { get; set; }
        public virtual DbSet<RequestsCards> RequestsCards { get; set; }
        public virtual DbSet<RequestsBanks> RequestsBanks { get; set; }
        public virtual DbSet<CustomerAccountMomentumBonus> CustomerAccountMomentumBonus { get; set; }
        public virtual DbSet<Talks> Talks { get; set; }
        public virtual DbSet<Talktag> Talktag { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<OpenCourseHistory> OpenCourseHistory { get; set; }
        public virtual DbSet<CourseType> CourseType { get; set; }
        public virtual DbSet<AboutCoures> AboutCoures { get; set; }
        public virtual DbSet<CourseTag> CourseTag { get; set; }
        public virtual DbSet<DiplomaInstructors> DiplomaInstructors { get; set; }
        public virtual DbSet<Employe> Employe { get; set; }
        public virtual DbSet<Media> Media { get; set; }
        public virtual DbSet<DiplomaDetails> DiplomaDetails { get; set; }
        public virtual DbSet<BalanceActionHistory> BalanceActionHistory { get; set; }
        public virtual DbSet<CourseCourseTagMapping> CourseCourseTagMapping { get; set; }
        public virtual DbSet<MetaTags> MetaTags { get; set; }
        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<PictureMapping> PictureMapping { get; set; }
        public virtual DbSet<dinamicday> Dinamicday { get; set; }
        public virtual DbSet<AboutContent> AboutContent { get; set; }
        public virtual DbSet<OnlineAttendance> OnlineAttendance { get; set; }
        public virtual DbSet<Language> Language { get; set; }
        public virtual DbSet<CourseLevel> Level { get; set; }
       
        public virtual DbSet<Mapdiscreptiioncoures> Mapdiscreptiioncoures { get; set; }

        public virtual DbSet<Video> Video { get; set; }
        public virtual DbSet<VideoMapping> VideoMapping { get; set; }
        public virtual DbSet<Allpathviewsql> Allpathviewsql { get; set; }

        public virtual DbSet<VidioStopAt> VidioStopAt { get; set; }
        public virtual DbSet<Tagcoures> Tagcoures { get; set; }
        public virtual DbSet<LiveCourseTrainning> LiveCourseTrainning { get; set; }


        #region Ticket Support register
        public virtual DbSet<Complaint> Complaint { get; set; }
        public virtual DbSet<ComplaintReply> ComplaintReply { get; set; }
        public virtual DbSet<ComplaintAttachment> ComplaintAttachment { get; set; }
        #endregion
   
        #region Ticket Support not register
        public virtual DbSet<TicketSupport> TicketSupport { get; set; }
        public virtual DbSet<TicketSupportReply> TicketSupportReply { get; set; }
        #endregion






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

          //  SeedCountries(modelBuilder);


            // Seed roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Inactive" },
                new Role { Id = 3, Name = "Active" },
                new Role { Id = 4, Name = "Instructor" }
            );

            modelBuilder.Entity<CustomerNetwork>()
        .HasOne(cn => cn.Child)
        .WithMany(ca => ca.CustomerNetworkChild)
        .HasForeignKey(cn => cn.ChildId)
        .OnDelete(DeleteBehavior.Restrict); // Remove cascade delete behavior

            modelBuilder.Entity<CustomerNetwork>()
       .HasOne(cn => cn.Parent)
       .WithMany(ca => ca.CustomerNetworkParent)
       .HasForeignKey(cn => cn.ParentId)
       .IsRequired(false)  // Allow null values
       .OnDelete(DeleteBehavior.Restrict);

            //    modelBuilder.Entity<CustomerAttribute>()
            //.HasMany(c => c.CustomerNetworkChild)  // Define the navigation property
            //.WithOne(n => n.Child)                 // Define the inverse navigation property
            //.HasForeignKey(n => n.ChildId);        // Define the foreign key

            //    modelBuilder.Entity<CustomerAttribute>()
            //        .HasMany(c => c.CustomerNetworkParent) // Define the navigation property
            //        .WithOne(n => n.Parent)                // Define the inverse navigation property
            //        .HasForeignKey(n => n.ParentId);

            modelBuilder.Entity<CustomerAttribute>()
        .OwnsOne(c => c.RefreshTokens);

            modelBuilder.Entity<Package>()
                       .Property(p => p.Summit_Cost)
                       .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PointEquation>()
       .Property(p => p.BalanceUSD)
       .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PointEquation>()
                .Property(p => p.Points)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<PointProcess>()
                .Property(p => p.Value)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Profit>()
                .Property(p => p.Profit1)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<RequestCash>()
                .Property(p => p.RequestedAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Talks>()
                .Property(p => p.timeduration)
                .HasColumnType("decimal(18,2)");


            base.OnModelCreating(modelBuilder);
        }




        private void SeedCountries(ModelBuilder modelBuilder)
        {
            string countriesJson;
            try
            {
                countriesJson = File.ReadAllText("countries.json");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Error: Could not find countries.json file.");
                throw ex;
            }

            List<Countries> countries;
            try
            {
                countries = JsonConvert.DeserializeObject<List<Countries>>(countriesJson);
            }
            catch (JsonException ex)
            {
                Console.WriteLine("Error: Could not parse countries.json as valid JSON.");
                throw ex;
            }

            // Assign negative IDs for seeding
            int id = +1;
            foreach (var country in countries)
            {
                country.Id = id++;
            }

            modelBuilder.Entity<Countries>().HasData(countries);
        }




    }
}

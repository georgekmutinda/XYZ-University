using Microsoft.AspNetCore.Identity;
using XYZUniversityAPI.Domain.Entities;

namespace XYZUniversityAPI.Infrastructure.Data
{
    public static class ClientSeed
    {
        public static void Seed(AppDbContext context)
        {
            SeedApiClients(context);
            SeedAdmin(context);
            SeedData(context); // seeds all other entities
        }

        private static void SeedApiClients(AppDbContext context)
        {
            if (context.Clients.Any(c => c.Role != "Admin"))
                return;

            var passwordHasher = new PasswordHasher<Client>();

            var clients = new List<Client>
            {
                new Client { ClientId = "AMwathi-001", ClientName = "Alpha Mwathi", ContactEmail = "alpha@gmail.com", CreatedBy = "George Kyalo", Role = "User", IsActive = true, TokenLifetimeMinutes = 60, CreatedAt = DateTime.UtcNow },
                new Client { ClientId = "LMshote-002", ClientName = "Lucious Mshote", ContactEmail = "lucious@gmail.com", CreatedBy = "George Kyalo", Role = "User", IsActive = true, TokenLifetimeMinutes = 60, CreatedAt = DateTime.UtcNow },
                new Client { ClientId = "MWeku-003", ClientName = "Marriam Weku", ContactEmail = "marriam@gmail.com", CreatedBy = "George Kyalo", Role = "User", IsActive = true, TokenLifetimeMinutes = 60, CreatedAt = DateTime.UtcNow },
                new Client { ClientId = "VCheru-004", ClientName = "Vivian Cheruiyot", ContactEmail = "vivian@gmail.com", CreatedBy = "George Kyalo", Role = "User", IsActive = true, TokenLifetimeMinutes = 60, CreatedAt = DateTime.UtcNow },
                new Client { ClientId = "CMwadime-005", ClientName = "Carol Mwadime", ContactEmail = "carol@gmail.com", CreatedBy = "George Kyalo", Role = "User", IsActive = true, TokenLifetimeMinutes = 60, CreatedAt = DateTime.UtcNow },
                new Client { ClientId = "MKawira-006", ClientName = "Mercy Kawira", ContactEmail = "mercy@gmail.com", CreatedBy = "George Kyalo", Role = "User", IsActive = true, TokenLifetimeMinutes = 60, CreatedAt = DateTime.UtcNow }
            };

            foreach (var client in clients)
            {
                var firstName = client.ClientName.Split(' ')[0];
                var rawPassword = firstName + "123";
                client.ClientSecretHash = passwordHasher.HashPassword(client, rawPassword);

                Console.WriteLine($"Client: {client.ClientName}, Password: {rawPassword}");
            }

            context.Clients.AddRange(clients);
            context.SaveChanges();
        }

        private static void SeedAdmin(AppDbContext context)
        {
            if (context.Clients.Any(c => c.Role == "Admin"))
                return;

            var admin = new Client
            {
                ClientId = "admin--001",
                ClientName = "System Administrator",
                Role = "Admin",
                ContactEmail = "adminmike@gmail.com",
                IsActive = true,
                TokenLifetimeMinutes = 10,
                CreatedAt = DateTime.UtcNow
            };

            var hasher = new PasswordHasher<Client>();
            var adminPassword = "new_admin001.";
            admin.ClientSecretHash = hasher.HashPassword(admin, adminPassword);

            Console.WriteLine($"Admin ClientId: {admin.ClientId}, Password: {adminPassword}");

            context.Clients.Add(admin);
            context.SaveChanges();
        }

        public static void SeedData(AppDbContext context)
        {
            // ------- Admins ----------
            if (!context.Admins.Any())
            {
                var admins = new List<Admin>
                {
                    new Admin { AdminName = "Alice Johnson" },
                    new Admin { AdminName = "Brian Smith" },
                    new Admin { AdminName = "Brian Muia" },
                    new Admin { AdminName = "Mercy Njogu" }
                };
                context.Admins.AddRange(admins);
                context.SaveChanges();
            }
            var adminsList = context.Admins.ToList();

            // -------------------- Courses --------------------
            if (!context.Courses.Any())
            {
                var courses = new List<Course>
                {
                    new Course { CourseName = "Computer Science",CourseFee = 330000 },
                    new Course { CourseName = "Information Technology",CourseFee = 310000 },
                    new Course { CourseName = "Software Engineering",CourseFee = 310000 },
                    new Course { CourseName = "Cybersecurity",CourseFee = 290000 },
                    new Course { CourseName = "Law",CourseFee = 450000 }
                };
                context.Courses.AddRange(courses);
                context.SaveChanges();
            }
            var coursesList = context.Courses.ToList();

            // -------------------- Payment Types --------------------
            if (!context.PaymentTypes.Any())
            {
                var paymentTypes = new List<PaymentType>
                {
                    new PaymentType { TypeName = "Cash" },
                    new PaymentType { TypeName = "Cheque" },
                    new PaymentType { TypeName = "Swift" },
                    new PaymentType { TypeName = "PesaLink" },
                    new PaymentType { TypeName = "RTGS" }
                };
                context.PaymentTypes.AddRange(paymentTypes);
                context.SaveChanges();
            }
            var paymentTypesList = context.PaymentTypes.ToList();

            // -------------------- Payment Channels --------------------
            if (!context.PaymentChannels.Any())
            {
                var paymentChannels = new List<PaymentChannel>
                {
                    new PaymentChannel { ChannelName = "Bank Transfer" },
                    new PaymentChannel { ChannelName = "Mobile Money" },
                    new PaymentChannel { ChannelName = "Credit/Debit Card" },
                    new PaymentChannel { ChannelName = "POS" }
                };
                context.PaymentChannels.AddRange(paymentChannels);
                context.SaveChanges();
            }
            var paymentChannelsList = context.PaymentChannels.ToList();

            // -------------------- Students --------------------
            if (!context.Students.Any())//only insert data if the table is empty
            {
                var students = new List<Student>
                {
                    new Student { AdmissionNumber = "CIT-001-2026", StudentId = 1, FirstName = "Agnes", LastName = "Munyiva", DateOfBirth = new DateTime(2005,3,12).ToUniversalTime(), EnrollmentDate = DateTime.UtcNow.AddMonths(-3), IsActive = true, IsValid = true, CreatedAt = DateTime.UtcNow, CreatedBy = adminsList[0].AdminId, CourseId = coursesList[0].CourseId },

                    new Student { AdmissionNumber = "CIT-002-2026", StudentId = 2, FirstName = "Mary", LastName = "Wanjiku", DateOfBirth = new DateTime(2003,7,8).ToUniversalTime(), EnrollmentDate = DateTime.UtcNow.AddMonths(-4), IsActive = true, IsValid = true, CreatedAt = DateTime.UtcNow, CreatedBy = adminsList[1].AdminId, CourseId = coursesList[1].CourseId },

                    new Student { AdmissionNumber = "CIT-003-2026", StudentId = 3, FirstName = "Alicia", LastName = "Munene", DateOfBirth = new DateTime(2006,8,8).ToUniversalTime(), EnrollmentDate = DateTime.UtcNow.AddMonths(-6), IsActive = true, IsValid = true, CreatedAt = DateTime.UtcNow, CreatedBy = adminsList[2].AdminId, CourseId = coursesList[2].CourseId },

                    new Student { AdmissionNumber = "CIT-004-2026", StudentId = 4, FirstName = "Colins", LastName = "Mutuku", DateOfBirth = new DateTime(2005,7,4).ToUniversalTime(), EnrollmentDate = DateTime.UtcNow.AddMonths(-6), IsActive = true, IsValid = true, CreatedAt = DateTime.UtcNow, CreatedBy = adminsList[2].AdminId, CourseId = coursesList[2].CourseId },

                    new Student { AdmissionNumber = "CIT-005-2026", StudentId = 5, FirstName = "Deo", LastName = "Martin", DateOfBirth = new DateTime(2006,3,5).ToUniversalTime(), EnrollmentDate = DateTime.UtcNow.AddMonths(-6), IsActive = false, IsValid = true, CreatedAt = DateTime.UtcNow, CreatedBy = adminsList[3].AdminId, CourseId = coursesList[3].CourseId },

                    new Student { AdmissionNumber = "CIT-006-2026", StudentId = 6, FirstName = "Linda", LastName = "Mueni", DateOfBirth = new DateTime(2005,7,8).ToUniversalTime(), EnrollmentDate = DateTime.UtcNow.AddMonths(-2), IsActive = true, IsValid = false, CreatedAt = DateTime.UtcNow, CreatedBy = adminsList[2].AdminId, CourseId = coursesList[2].CourseId },

                    new Student { AdmissionNumber = "CIT-007-2026", StudentId = 7, FirstName = "Elizabeth", LastName = "Wambua", DateOfBirth = new DateTime(2004,4,1).ToUniversalTime(), EnrollmentDate = DateTime.UtcNow.AddMonths(-2), IsActive = true, IsValid = true, CreatedAt = DateTime.UtcNow, CreatedBy = adminsList[3].AdminId, CourseId = coursesList[3].CourseId }
                };
                context.Students.AddRange(students);
                context.SaveChanges();
            }
            var studentsList = context.Students.ToList();

            // -------------------- Student Contacts --------------------
            if (!context.StudentContacts.Any())
            {
                var studentContacts = new List<StudentContact>
                {
                    new StudentContact { AdmissionNumber = studentsList[0].AdmissionNumber, Email = "agnesmunyiva@xyzuni.edu", Phone = "0712345678" },
                    new StudentContact { AdmissionNumber = studentsList[1].AdmissionNumber, Email = "marywanjiku@xyzuni.edu", Phone = "0798765432" },
                    new StudentContact { AdmissionNumber = studentsList[2].AdmissionNumber, Email = "amunene@gmail.com", Phone = "0756748312" },
                    new StudentContact { AdmissionNumber = studentsList[3].AdmissionNumber, Email = "cmutuku@gmail.com", Phone = "0735665432" },
                    new StudentContact { AdmissionNumber = studentsList[4].AdmissionNumber, Email = "dmartin@gmail.com", Phone = "0789756432" },
                    new StudentContact { AdmissionNumber = studentsList[5].AdmissionNumber, Email = "lmueni@gmail.com", Phone = "0792460535" },
                    new StudentContact { AdmissionNumber = studentsList[6].AdmissionNumber, Email = "ewambua@gmail.com", Phone = "0715105548" }
                };
                context.StudentContacts.AddRange(studentContacts);
                context.SaveChanges();
            }

            // -------------------- Payments --------------------
            if (!context.Payments.Any())
            {
                var payments = new List<Payment>
                {
                    new Payment
                    {
                        ReferenceNumber = "PAY-20260128-001",
                        AdmissionNumber = studentsList[0].AdmissionNumber,
                        Amount = 1500.00m,
                        PaidBy = "Lucious Mshote",
                        PaymentTypeId = paymentTypesList.First(p => p.TypeName=="Cheque").PaymentTypeId,
                        PaymentChannelId = paymentChannelsList.First(c => c.ChannelName=="Mobile Money").PaymentChannelId,
                        PaymentDate = DateTime.UtcNow.AddDays(-10),
                        CreatedAt = DateTime.UtcNow
                    },
                    new Payment
                    {
                        ReferenceNumber = "PAY-20260128-002",
                        AdmissionNumber = studentsList[1].AdmissionNumber,
                        Amount = 1200.00m,
                        PaidBy = "Kibaba Banking",
                        PaymentTypeId = paymentTypesList.First(p => p.TypeName=="Swift").PaymentTypeId,
                        PaymentChannelId = paymentChannelsList.First(c => c.ChannelName=="Credit/Debit Card").PaymentChannelId,
                        PaymentDate = DateTime.UtcNow.AddDays(-7),
                        CreatedAt = DateTime.UtcNow
                    }
                };
                context.Payments.AddRange(payments);
                context.SaveChanges();
            }

            Console.WriteLine("✅ Database seeded with Admins, Courses, Students, Payments, and Contacts.");
        }
    }
}

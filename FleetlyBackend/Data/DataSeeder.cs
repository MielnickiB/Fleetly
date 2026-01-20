using Fleetly.Shared.Enums;
using FleetlyBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Data
{
    public static class DbSeeder
    {
        private static readonly Random _random = new();

        private static readonly Dictionary<string, List<string>> _namesAndSurnames = new()
        {
            { "Jan", new List<string> { "Kowalski", "Nowak", "Wiśniewski", "Wójcik", "Kowalczyk" } },
            { "Anna", new List<string> { "Kamińska", "Lewandowska", "Dąbrowska", "Zielińska", "Szymańska" } },
            { "Piotr", new List<string> { "Woźniak", "Kozłowski", "Jankowski", "Mazur", "Kwiatkowski" } },
            { "Katarzyna", new List<string> { "Krawczyk", "Kaczmarek", "Piotrowska", "Grabowska", "Pawłowska" } },
            { "Tomasz", new List<string> { "Michalski", "Nowicki", "Adamczyk", "Duda", "Zając" } },
            { "Mateusz", new List<string> { "Król", "Wieczorek", "Jabłoński", "Majewski", "Olszewski" } },
            { "Magdalena", new List<string> { "Stępień", "Dudek", "Wróbel", "Pawlak", "Sikora" } },
            { "Agnieszka", new List<string> { "Baran", "Rutkowska", "Gajda", "Czarnecka", "Włodarczyk" } },
            { "Łukasz", new List<string> { "Sawicki", "Bąk", "Szczepański", "Lis", "Wilk" } },
            { "Joanna", new List<string> { "Zawadzka", "Kubiak", "Witkowska", "Walczak", "Sadowska" } },
            { "Marcin", new List<string> { "Kucharski", "Górski", "Urbański", "Chmielowski", "Cieślak" } },
            { "Monika", new List<string> { "Sikorska", "Wysocka", "Kalinowska", "Błaszczyk", "Makowska" } }
        };

        private static readonly List<string> _companies = new()
        {
            "Tech Solutions", "AutoRentals", "LogiTrans", "FleetManage", "DriveEasy",
            "CityCars", "MoveIt", "QuickRent", "UrbanFleet", "ProDrive"
        };

        private static readonly List<string> _vehicleDetails = new()
        {
            "Pojazd służbowy", "Auto firmowe", "Flota korporacyjna", "Samochód do zadań specjalnych",
            "Pojazd operacyjny", "Auto dla pracowników", "Flota wynajmowana", "Samochód do celów służbowych"
        };

        private record CityData(string City, string ZipPrefix, List<string> Streets);

        private static readonly List<CityData> _realLocations = new()
        {
            new("Warszawa", "00", new() { "Marszałkowska", "Aleje Jerozolimskie", "Mokotowska", "Wspólna", "Złota", "Chmielna" }),
            new("Kraków", "30", new() { "Floriańska", "Grodzka", "Szewska", "Długa", "Karmelicka", "Zwierzyniecka" }),
            new("Wrocław", "50", new() { "Świdnicka", "Oławska", "Ruska", "Legnicka", "Grabiszyńska", "Piłsudskiego" }),
            new("Poznań", "60", new() { "Święty Marcin", "Półwiejska", "Głogowska", "Dąbrowskiego", "Bukowska" }),
            new("Gdańsk", "80", new() { "Długa", "Mariacka", "Piwna", "Grunwaldzka", "Wały Jagiellońskie" }),
            new("Łódź", "90", new() { "Piotrkowska", "Kościuszki", "Narutowicza", "Sienkiewicza", "Zachodnia" }),
            new("Katowice", "40", new() { "3 Maja", "Chorzowska", "Mariacka", "Korfantego", "Francuska" }),
            new("Lublin", "20", new() { "Krakowskie Przedmieście", "Lipowa", "Narutowicza", "Głęboka" })
        };

        private static readonly Dictionary<string, List<string>> _carData = new()
        {
            { "Toyota", new List<string> { "Corolla", "Yaris", "RAV4", "C-HR", "Camry" } },
            { "Skoda", new List<string> { "Octavia", "Fabia", "Superb", "Karoq", "Kodiaq" } },
            { "Volkswagen", new List<string> { "Golf", "Passat", "Tiguan", "Polo", "Arteon" } },
            { "Kia", new List<string> { "Sportage", "Ceed", "Rio", "Stonic", "Xceed" } },
            { "Hyundai", new List<string> { "Tucson", "i30", "i20", "Kona", "Elantra" } },
            { "BMW", new List<string> { "Seria 3", "Seria 5", "X3", "X5", "Seria 1" } },
            { "Mercedes-Benz", new List<string> { "Klasa A", "Klasa C", "Klasa E", "GLC", "GLE" } },
            { "Audi", new List<string> { "A3", "A4", "A6", "Q3", "Q5" } },
            { "Ford", new List<string> { "Focus", "Mondeo", "Kuga", "Fiesta", "Puma" } },
            { "Renault", new List<string> { "Clio", "Megane", "Captur", "Arkana", "Austral" } }
        };

        public static async Task SeedAsync(FleetlyContext context, IPasswordHasher<User> passwordHasher)
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync(context);
            await SeedCostLimitsAsync(context);
            await SeedBrandsAndModelsAsync(context);
            await SeedUsersAsync(context, passwordHasher);
            await SeedOrdersAsync(context);
        }

        private static async Task SeedRolesAsync(FleetlyContext context)
        {
            if (!await context.UserRoles.AnyAsync())
            {
                var roles = new List<UserRole>
                {
                    new() { RoleName = "Admin" },
                    new() { RoleName = "Client" },
                    new() { RoleName = "Worker" }
                };
                await context.UserRoles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedCostLimitsAsync(FleetlyContext context)
        {
            if (!await context.CostLimits.AnyAsync())
            {
                var limits = new List<CostLimit>();
                decimal baseSalary = 100m;
                decimal costBase = 30m;

                for (int i = 0; i < 10; i++)
                {
                    int minKm = i * 100 + (i == 0 ? 0 : 1);
                    int maxKm = (i + 1) * 100;

                    limits.Add(new CostLimit
                    {
                        RangeOfKmMin = minKm,
                        RangeOfKmMax = maxKm,
                        BaseSalary = baseSalary,
                        MaxSalary = baseSalary + 50m,
                        MaxCosts = costBase,
                        IsActive = true
                    });

                    baseSalary += 50m;
                    costBase += 10m;
                }
                await context.CostLimits.AddRangeAsync(limits);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedBrandsAndModelsAsync(FleetlyContext context)
        {
            if (!await context.CarBrands.AnyAsync())
            {
                foreach (var brandEntry in _carData)
                {
                    var brand = new CarBrand
                    {
                        BrandName = brandEntry.Key,
                        IsActive = true,
                        Models = new List<BrandModel>()
                    };

                    foreach (var modelName in brandEntry.Value)
                    {
                        brand.Models.Add(new BrandModel
                        {
                            ModelName = modelName,
                            IsActive = true
                        });
                    }
                    await context.CarBrands.AddAsync(brand);
                }
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUsersAsync(FleetlyContext context, IPasswordHasher<User> passwordHasher)
        {
            var adminRole = await context.UserRoles.FirstAsync(r => r.RoleName == "Admin");
            var clientRole = await context.UserRoles.FirstAsync(r => r.RoleName == "Client");
            var workerRole = await context.UserRoles.FirstAsync(r => r.RoleName == "Worker");

            var allModels = await context.BrandModels.Include(m => m.CarBrand).ToListAsync();

            var createdEmails = new HashSet<string>();

            var existingDbEmails = await context.Users.Select(u => u.Email).ToListAsync();
            foreach (var dbEmail in existingDbEmails) createdEmails.Add(dbEmail.ToLower());

            if (!await context.Users.AnyAsync(u => u.Email == "user@example.com"))
            {
                var admins = new List<User>
                {
                    CreateUser(adminRole.Id, "user@example.com", "System", "Admin", "Fleetly HQ", passwordHasher),
                    CreateUser(adminRole.Id, "support@fleetly.com", "Helpdesk", "Support", "Fleetly Ops", passwordHasher)
                };
                await context.Users.AddRangeAsync(admins);
            }

            if (!await context.Users.AnyAsync(u => u.Role.RoleName == "Worker"))
            {
                var workers = new List<User>();
                for (int i = 0; i < 30; i++)
                {
                    var (name, surname) = GetRandomName();
                    var email = GenerateUniqueEmail(name, surname, "fleetly.com", createdEmails);
                    createdEmails.Add(email.ToLower());

                    workers.Add(CreateUser(workerRole.Id, email, name, surname, null, passwordHasher));
                }
                try
                {
                    await context.Users.AddRangeAsync(workers);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Błąd przy zapisie pracowników: {ex.InnerException?.Message ?? ex.Message}");
                }
            }

            if (!await context.Users.AnyAsync(u => u.Role.RoleName == "Client"))
            {
                var clients = new List<User>();

                var availableCompanies = new Queue<string>(_companies.OrderBy(x => _random.Next()));

                for (int i = 0; i < 10; i++)
                {
                    var (name, surname) = GetRandomName();
                    string company = availableCompanies.Count > 0 ? availableCompanies.Dequeue() : "Firma";
                    string domain = SanitizeText(company.Replace(" ", "")) + ".pl";
                    string email = GenerateUniqueEmail(name, surname, domain, createdEmails);

                    var client = CreateUser(clientRole.Id, email, name, surname, company, passwordHasher);

                    int locCount = _random.Next(5, 11);
                    for (int j = 0; j < locCount; j++)
                    {
                        client.Locations.Add(CreateRealLocation());
                    }

                    int carCount = _random.Next(2, 11);
                    for (int k = 0; k < carCount; k++)
                    {
                        var randomModel = allModels[_random.Next(allModels.Count)];
                        client.Vehicles.Add(CreateRandomVehicle(randomModel));
                    }

                    clients.Add(client);
                }
                try
                {
                    await context.Users.AddRangeAsync(clients);
                    await context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Błąd przy zapisie klientów: {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }

        private static async Task SeedOrdersAsync(FleetlyContext context)
        {
            if (await context.Orders.AnyAsync()) return;

            var clients = await context.Users
                .Where(u => u.Role.RoleName == "Client")
                .Include(u => u.Vehicles)
                .Include(u => u.Locations)
                .Include(u => u.Role)
                .ToListAsync();

            var workers = await context.Users
                .Where(u => u.Role.RoleName == "Worker")
                .Include(u => u.Role)
                .ToListAsync();

            var costLimits = await context.CostLimits.OrderBy(x => x.RangeOfKmMin).ToListAsync();

            var orders = new List<Order>();

            foreach (var client in clients)
            {
                int ordersCount = _random.Next(5, 9);

                for (int i = 0; i < ordersCount; i++)
                {
                    if (client.Vehicles.Count == 0 || client.Locations.Count < 2) continue;

                    var vehicle = client.Vehicles.ElementAt(_random.Next(client.Vehicles.Count));

                    int? assignedWorkerId = null;

                    var startLocation = client.Locations.ElementAt(_random.Next(client.Locations.Count));

                    var availableEndLocations = client.Locations.Where(l => l.Id != startLocation.Id).ToList();

                    if (availableEndLocations.Count == 0) continue;

                    var endLocation = availableEndLocations[_random.Next(availableEndLocations.Count)];

                    var dateOffset = _random.Next(-10, 46);
                    var startTime = DateTime.Now.AddDays(dateOffset).AddHours(_random.Next(8, 16));
                    var deadline = startTime.AddHours(_random.Next(4, 24));

                    OrderStatus status;
                    DateTime? actualStart = null;
                    DateTime? actualEnd = null;

                    if (dateOffset < -1)
                    {
                        status = OrderStatus.ApprovedByAdmin;
                        actualStart = startTime.AddMinutes(_random.Next(-15, 30));
                        actualEnd = actualStart.Value.AddHours(_random.Next(2, 8));

                        var worker = workers[_random.Next(workers.Count)];
                        assignedWorkerId = worker.Id;
                    }
                    else if (dateOffset <= 5)
                    {
                        status = OrderStatus.Created;
                    }
                    else
                    {
                        status = OrderStatus.PendingApproval;
                    }

                    int distance = _random.Next(50, 950);
                    var limit = costLimits.FirstOrDefault(l => distance >= l.RangeOfKmMin && distance <= l.RangeOfKmMax)
                                ?? costLimits.Last();

                    var contactPerson = GetRandomName();

                    var order = new Order
                    {
                        ClientId = client.Id,

                        WorkerId = assignedWorkerId,

                        VehicleId = vehicle.Id,

                        StartLocation = startLocation,
                        EndLocation = endLocation,

                        CostLimitId = limit.Id,
                        RangeOfKm = distance,
                        Salary = limit.BaseSalary,
                        FuelCosts = Math.Round(limit.MaxCosts * 0.7m, 2),
                        AdditionalCosts = 0m,
                        Expenses = new List<Expense>(),

                        Status = status,
                        Details = _random.Next(0, 2) == 0 ? "Proszę o kontakt przed przyjazdem" : null,

                        EndContactName = $"{contactPerson.Name} {contactPerson.Surname}",
                        EndContactPhone = $"600{_random.Next(100000, 999999)}",

                        StartTime = startTime,
                        Deadline = deadline,
                        ActualStartTime = actualStart,
                        ActualEndTime = actualEnd,

                        IsActive = true
                    };

                    orders.Add(order);
                }
            }

            try
            {
                await context.Orders.AddRangeAsync(orders);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Błąd dodawania zleceń: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private static User CreateUser(int roleId, string email, string name, string surname, string? company, IPasswordHasher<User> passwordHasher)
        {
            var user = new User
            {
                RoleId = roleId,
                Email = email,
                IsActive = true,
                PasswordHash = string.Empty,
                Details = new UserDetails
                {
                    Name = name,
                    Surname = surname,
                    Company = company,
                    PhoneNumber = $"500{_random.Next(100000, 999999)}"
                }
            };
            user.PasswordHash = passwordHasher.HashPassword(user, "string");
            return user;
        }

        private static Location CreateRealLocation()
        {
            var cityData = _realLocations[_random.Next(_realLocations.Count)];

            string zip = $"{cityData.ZipPrefix}-{_random.Next(100, 999)}";

            string street = cityData.Streets[_random.Next(cityData.Streets.Count)];

            return new Location
            {
                City = cityData.City,
                Street = street,
                PostalCode = zip,
                BuildingNumber = _random.Next(1, 150).ToString(),
                ApartmentNumber = _random.Next(0, 3) == 0 ? null : _random.Next(1, 50).ToString(),
                Description = _random.Next(0, 4) == 0 ? "Wejście od ulicy" : null,
                IsPublic = _random.Next(0, 2) == 1,
                IsActive = true
            };
        }

        private static Vehicle CreateRandomVehicle(BrandModel model)
        {
            string reg = $"W{GetRandomLetter()}{GetRandomLetter()} {_random.Next(10000, 99999)}";
            string details = _vehicleDetails[_random.Next(_vehicleDetails.Count)];

            return new Vehicle
            {
                BrandModelId = model.Id,
                RegistrationNumber = reg,
                Mileage = _random.Next(5000, 200000),
                VIN = GenerateRandomVIN(),
                Year = _random.Next(2018, 2024),
                Details = details,
                FuelType = (FuelType)_random.Next(0, 6),
                IsActive = true
            };
        }

        private static (string Name, string Surname) GetRandomName()
        {
            var keys = _namesAndSurnames.Keys.ToList();
            string name = keys[_random.Next(keys.Count)];

            var surnames = _namesAndSurnames[name];
            string surname = surnames[_random.Next(surnames.Count)];

            return (name, surname);
        }

        private static string GenerateUniqueEmail(string name, string surname, string domain, HashSet<string> existingEmails)
        {
            string cleanName = SanitizeText(name);
            string cleanSurname = SanitizeText(surname);
            string baseEmail = $"{cleanName}.{cleanSurname}@{domain}".ToLower();

            if (!existingEmails.Contains(baseEmail))
            {
                return baseEmail;
            }

            int counter = 1;
            while (true)
            {
                string newEmail = $"{cleanName}.{cleanSurname}{counter}@{domain}".ToLower();
                if (!existingEmails.Contains(newEmail))
                {
                    return newEmail;
                }
                counter++;
            }
        }

        private static string SanitizeText(string text)
        {
            return text.ToLower()
                .Replace("ą", "a").Replace("ć", "c").Replace("ę", "e")
                .Replace("ł", "l").Replace("ń", "n").Replace("ó", "o")
                .Replace("ś", "s").Replace("ź", "z").Replace("ż", "z");
        }

        private static string GenerateRandomVIN()
        {
            const string chars = "ABCDEFGHJKLMNPRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 17)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private static char GetRandomLetter()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return chars[_random.Next(chars.Length)];
        }
    }
}
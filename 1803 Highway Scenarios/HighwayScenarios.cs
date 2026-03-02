using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using GTA;
using GTA.Native;
using GTA.Math;
using System.Windows.Forms;

namespace HighwayScenarios_1803
{
    public class HighwayScenarios : Script
    {
        // Configuration
        private Config config;

        // All locations (each can spawn any scenario type)
        private List<ScenarioLocation> allLocations;

        // Active scenarios
        private List<ActiveScenario> activeScenarios;

        // Random generator
        private Random random = new Random();

        // Update timer
        private int lastUpdate = 0;
        private const int UPDATE_INTERVAL = 30000; // Check every 30 seconds

        public HighwayScenarios()
        {
            Tick += OnTick;
            Interval = 100;

            activeScenarios = new List<ActiveScenario>();

            LoadConfig();
            InitializeLocations();

            GTA.UI.Notification.PostTicker("~b~1803 Highway Scenarios ~w~loaded! ~g~Auto-spawning active", false);
        }

        private void InitializeLocations()
        {
            allLocations = new List<ScenarioLocation>();

            // ============ ALL LOCATIONS WITH HEADINGS ============

            // PALETO BAY AREA (4 locations)
            allLocations.Add(new ScenarioLocation(new Vector3(484.21f, 6576.69f, 26.70f), 88.32f, "Paleto Bay 1"));
            allLocations.Add(new ScenarioLocation(new Vector3(-674.10f, 5539.72f, 37.89f), 304.78f, "Paleto Bay 2"));
            allLocations.Add(new ScenarioLocation(new Vector3(1929.91f, 6284.47f, 42.34f), 206.60f, "Paleto Bay 3"));
            allLocations.Add(new ScenarioLocation(new Vector3(2404.25f, 5800.16f, 45.65f), 33.84f, "Paleto Bay 4"));

            // NORTH CHUMASH AREA (3 locations)
            allLocations.Add(new ScenarioLocation(new Vector3(-2455.80f, 3725.83f, 15.83f), 347.12f, "North Chumash 1"));
            allLocations.Add(new ScenarioLocation(new Vector3(-2275.62f, 4245.20f, 43.32f), 149.57f, "North Chumash 2"));
            allLocations.Add(new ScenarioLocation(new Vector3(-3140.68f, 908.63f, 14.18f), 5.60f, "North Chumash 3"));

            // LAGO ZANCUDO AREA (3 locations)
            allLocations.Add(new ScenarioLocation(new Vector3(-2627.62f, 2922.35f, 16.40f), 175.27f, "Lago Zancudo 1"));
            allLocations.Add(new ScenarioLocation(new Vector3(2471.31f, 2929.05f, 40.33f), 310.99f, "Lago Zancudo 2"));
            allLocations.Add(new ScenarioLocation(new Vector3(1995.04f, 2598.59f, 54.06f), 141.76f, "Lago Zancudo 3"));

            // SENORA FREEWAY (2 locations)
            allLocations.Add(new ScenarioLocation(new Vector3(2940.40f, 4010.55f, 51.10f), 10.96f, "Senora Fwy 1"));
            allLocations.Add(new ScenarioLocation(new Vector3(2892.37f, 4027.71f, 50.89f), 199.45f, "Senora Fwy 2"));

            // MOUNT GORDO (1 location)
            allLocations.Add(new ScenarioLocation(new Vector3(2591.48f, 520.36f, 44.49f), 181.99f, "Mount Gordo"));

            // LS FREEWAY (3 locations)
            allLocations.Add(new ScenarioLocation(new Vector3(699.51f, -186.70f, 46.50f), 338.18f, "LS Fwy 1"));
            allLocations.Add(new ScenarioLocation(new Vector3(1300.75f, 573.51f, 79.92f), 322.24f, "LS Fwy 2"));
            allLocations.Add(new ScenarioLocation(new Vector3(1732.05f, 1569.41f, 84.16f), 347.38f, "LS Fwy 3"));

            // PALOMINO FREEWAY (6 locations)
            allLocations.Add(new ScenarioLocation(new Vector3(1815.76f, 2193.65f, 53.60f), 170.97f, "Palomino Fwy 1"));
            allLocations.Add(new ScenarioLocation(new Vector3(2088.07f, 1382.91f, 75.11f), 213.62f, "Palomino Fwy 2"));
            allLocations.Add(new ScenarioLocation(new Vector3(2375.49f, -270.02f, 84.48f), 150.01f, "Palomino Fwy 3"));
            allLocations.Add(new ScenarioLocation(new Vector3(1529.60f, -1025.42f, 57.31f), 303.48f, "Palomino Fwy 4"));
            allLocations.Add(new ScenarioLocation(new Vector3(2477.27f, -136.99f, 89.21f), 335.19f, "Palomino Fwy 5"));
            allLocations.Add(new ScenarioLocation(new Vector3(2124.81f, 1381.37f, 75.00f), 39.03f, "Palomino Fwy 6"));

            // ELYSIAN FIELDS (1 location)
            allLocations.Add(new ScenarioLocation(new Vector3(1098.81f, -1780.63f, 28.81f), 206.80f, "Elysian Fields"));

            // DOWNTOWN HIGHWAYS (5 locations)
            allLocations.Add(new ScenarioLocation(new Vector3(865.21f, -670.09f, 42.74f), 58.99f, "Downtown 1"));
            allLocations.Add(new ScenarioLocation(new Vector3(-59.44f, -484.27f, 31.70f), 94.77f, "Downtown 2"));
            allLocations.Add(new ScenarioLocation(new Vector3(-1613.20f, -756.05f, 11.15f), 248.35f, "Downtown 3"));
            allLocations.Add(new ScenarioLocation(new Vector3(-310.73f, -538.73f, 24.86f), 279.10f, "Downtown 4"));
            allLocations.Add(new ScenarioLocation(new Vector3(-407.70f, -1506.49f, 37.01f), 353.14f, "Downtown 5"));

            // PACIFIC BLUFFS (1 location)
            allLocations.Add(new ScenarioLocation(new Vector3(-2980.97f, 100.57f, 13.82f), 237.33f, "Pacific Bluffs"));

            // Shuffle locations for random distribution
            ShuffleLocations(allLocations);
        }

        private void ShuffleLocations(List<ScenarioLocation> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                ScenarioLocation value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void LoadConfig()
        {
            string configPath = "scripts/1803 Highway Scenarios.ini";

            // Create directory if it doesn't exist
            string directory = Path.GetDirectoryName(configPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            config = new Config(configPath);

            if (!File.Exists(configPath))
            {
                CreateDefaultConfig(configPath);
            }
        }

        private void CreateDefaultConfig(string path)
        {
            string[] defaultConfig = new string[]
            {
                "[SpawnPercentages]",
                "TowTruck=25",
                "BrokenDown=25",
                "Crash=25",
                "Nothing=25",
                "",
                "[SpawnSettings]",
                "SpawnDistance=0.5", // Miles
                "DespawnDistance=0.7", // Miles
                "CheckRadius=5", // Feet
                "MaxConcurrentScenarios=3",
                "",
                "[ScenarioSettings]",
                "TowTruckDuration=600000", // 10 min
                "BrokenDownDuration=600000", // 10 min
                "CrashDuration=600000", // 10 min
                "",
                "[VehicleColors]",
                "TowTruckPrimary=88", // Yellow
                "TowTruckSecondary=0", // Black
                "BrokenDownPrimary=64", // Blue
                "CrashVehicle1Primary=27", // Red
                "CrashVehicle2Primary=0", // Black
            };

            File.WriteAllLines(path, defaultConfig);
        }

        private void OnTick(object sender, EventArgs e)
        {
            if (Game.GameTime - lastUpdate < UPDATE_INTERVAL) return;
            lastUpdate = Game.GameTime;

            Ped playerPed = Game.Player.Character;
            if (playerPed == null || !playerPed.Exists()) return;

            Vector3 playerPos = playerPed.Position;

            // Check for despawning scenarios
            CheckForDespawn(playerPos);

            // Check for spawning new scenarios
            CheckForSpawn(playerPos);
        }

        private void CheckForDespawn(Vector3 playerPos)
        {
            float despawnDistanceSq = (float)Math.Pow(config.DespawnDistance * 1609.34f, 2);

            for (int i = activeScenarios.Count - 1; i >= 0; i--)
            {
                ActiveScenario scenario = activeScenarios[i];

                float distSq = Vector3.DistanceSquared(playerPos, scenario.Location);

                if (distSq > despawnDistanceSq || Game.GameTime > scenario.DespawnTime)
                {
                    scenario.Cleanup();
                    activeScenarios.RemoveAt(i);
                }
            }
        }

        private void CheckForSpawn(Vector3 playerPos)
        {
            // Check if we can spawn more scenarios
            if (activeScenarios.Count >= config.MaxConcurrentScenarios) return;

            float spawnDistanceSq = (float)Math.Pow(config.SpawnDistance * 1609.34f, 2);

            foreach (var location in allLocations)
            {
                // Skip if location is on cooldown
                if (location.LastSpawnTime > 0 && Game.GameTime - location.LastSpawnTime < 300000) // 5 minute cooldown
                    continue;

                // Check distance from player
                float distToPlayerSq = Vector3.DistanceSquared(playerPos, location.Position);
                if (distToPlayerSq > spawnDistanceSq) continue;

                // Check if location is clear
                if (!IsLocationClear(location.Position)) continue;

                // Check spawn chance based on percentages
                if (ShouldSpawnScenario(out ScenarioType scenarioType))
                {
                    SpawnScenario(scenarioType, location);
                    break; // Only spawn one scenario per check
                }
            }
        }

        private bool IsLocationClear(Vector3 position)
        {
            float checkRadius = config.CheckRadius * 0.3048f; // Convert feet to meters

            // Check for other scenarios
            foreach (var scenario in activeScenarios)
            {
                if (Vector3.Distance(scenario.Location, position) < checkRadius)
                    return false;
            }

            // Check for vehicles
            Vehicle[] vehicles = World.GetNearbyVehicles(position, checkRadius);
            if (vehicles.Length > 0) return false;

            // Check for peds
            Ped[] peds = World.GetNearbyPeds(position, checkRadius);
            if (peds.Length > 1) return false; // Allow player, but not others

            return true;
        }

        private bool ShouldSpawnScenario(out ScenarioType scenarioType)
        {
            scenarioType = ScenarioType.TowTruck;
            int roll = random.Next(1, 101); // 1-100

            if (roll <= config.TowTruckChance)
            {
                scenarioType = ScenarioType.TowTruck;
                return true;
            }
            else if (roll <= config.TowTruckChance + config.BrokenDownChance)
            {
                scenarioType = ScenarioType.BrokenDown;
                return true;
            }
            else if (roll <= config.TowTruckChance + config.BrokenDownChance + config.CrashChance)
            {
                scenarioType = ScenarioType.Crash;
                return true;
            }

            return false; // Nothing spawns
        }

        private void SpawnScenario(ScenarioType type, ScenarioLocation location)
        {
            ActiveScenario scenario = null;

            try
            {
                switch (type)
                {
                    case ScenarioType.TowTruck:
                        scenario = SpawnTowTruckScenario(location);
                        break;
                    case ScenarioType.BrokenDown:
                        scenario = SpawnBrokenDownScenario(location);
                        break;
                    case ScenarioType.Crash:
                        scenario = SpawnCrashScenario(location);
                        break;
                }

                if (scenario != null)
                {
                    activeScenarios.Add(scenario);
                    location.LastSpawnTime = Game.GameTime;
                }
            }
            catch
            {
                // Silent fail - don't show errors to player
                scenario?.Cleanup();
            }
        }

        private ActiveScenario SpawnTowTruckScenario(ScenarioLocation location)
        {
            Vector3 pos = location.Position;
            float heading = location.Heading;

            // Calculate offset positions based on heading
            float angle = heading * (float)Math.PI / 180f;
            Vector3 forward = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle), 0);
            Vector3 right = new Vector3((float)Math.Cos(angle), -(float)Math.Sin(angle), 0);

            // Create tow truck (front vehicle)
            Vehicle towTruck = World.CreateVehicle(VehicleHash.Flatbed, pos);
            towTruck.Heading = heading;

            // Set colors using native functions
            Function.Call(Hash.SET_VEHICLE_COLOURS, towTruck, config.TowTruckPrimaryColor, config.TowTruckSecondaryColor);

            towTruck.Mods.LicensePlate = "TOW 803";
            Function.Call(Hash.SET_VEHICLE_SIREN, towTruck, true);
            Function.Call(Hash.SET_VEHICLE_LIGHTS, towTruck, 2);
            Function.Call(Hash.SET_VEHICLE_ENGINE_ON, towTruck, true, true);

            // Create broken down car BEHIND the tow truck
            Vector3 carPos = pos - forward * 8f; // 8 meters behind the tow truck
            Vehicle brokenCar = World.CreateVehicle(VehicleHash.Sentinel, carPos);
            brokenCar.Heading = heading; // Same heading as tow truck

            // Set color using native function
            Function.Call(Hash.SET_VEHICLE_COLOURS, brokenCar, 115, 115); // GreySilver equivalent

            brokenCar.EngineHealth = 200.0f;
            brokenCar.IsEngineRunning = false;
            Function.Call(Hash.SET_VEHICLE_ENGINE_HEALTH, brokenCar, 200.0f);
            Function.Call(Hash.SET_VEHICLE_UNDRIVEABLE, brokenCar, true);

            // Create tow truck driver INSIDE the truck (driver seat)
            Ped driver = World.CreatePed(PedHash.Tourist01AFM, pos + forward * 1f + right * 1f);
            driver.SetIntoVehicle(towTruck, VehicleSeat.Driver);
            driver.Task.LookAt(brokenCar);
            Function.Call(Hash.SET_PED_CONFIG_FLAG, driver, 342, true);

            // Create civilian standing VERY close to their broken car (right next to driver door)
            Vector3 civilianPos = carPos + right * 1.5f; // Stand to the side of the car, near driver door
            Ped civilian = World.CreatePed(PedHash.Business01AFY, civilianPos);
            civilian.Heading = heading + 90f; // Face towards the car
            civilian.Task.LookAt(brokenCar);
            Function.Call(Hash.SET_PED_CONFIG_FLAG, civilian, 342, true);

            // Make civilian look like they're on their phone (stranded motorist calling for help)
            Function.Call(Hash.TASK_START_SCENARIO_IN_PLACE, civilian, "WORLD_HUMAN_STAND_MOBILE", 0, true);

            return new ActiveScenario
            {
                Type = ScenarioType.TowTruck,
                Vehicles = new List<Vehicle> { towTruck, brokenCar },
                Peds = new List<Ped> { driver, civilian },
                Location = pos,
                SpawnTime = Game.GameTime,
                DespawnTime = Game.GameTime + config.TowTruckDuration,
                LocationName = location.Name
            };
        }

        private ActiveScenario SpawnBrokenDownScenario(ScenarioLocation location)
        {
            Vector3 pos = location.Position;
            float heading = location.Heading;

            // Create broken down car
            Vehicle brokenCar = World.CreateVehicle(VehicleHash.Tailgater, pos);
            brokenCar.Heading = heading;

            // Set color using native function
            Function.Call(Hash.SET_VEHICLE_COLOURS, brokenCar, config.BrokenDownPrimaryColor, config.BrokenDownPrimaryColor);

            brokenCar.EngineHealth = 150.0f;
            brokenCar.IsEngineRunning = false;
            Function.Call(Hash.SET_VEHICLE_ENGINE_HEALTH, brokenCar, 150.0f);
            Function.Call(Hash.SET_VEHICLE_UNDRIVEABLE, brokenCar, true);

            // Open hood
            Function.Call(Hash.SET_VEHICLE_DOOR_OPEN, brokenCar, 4, false, false);

            // Calculate offset based on heading
            float angle = heading * (float)Math.PI / 180f;
            Vector3 forward = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle), 0);
            Vector3 right = new Vector3((float)Math.Cos(angle), -(float)Math.Sin(angle), 0);

            // Create civilian
            Ped civilian = World.CreatePed(PedHash.Business01AFY, pos + forward * 3f + right * 2f);
            civilian.Heading = heading + 90f;

            // Make civilian use phone scenario
            Function.Call(Hash.TASK_USE_NEAREST_SCENARIO_TO_COORD, civilian, pos.X, pos.Y, pos.Z, 5f, 0);

            // Add hazard lights - using TOGGLE_VEHICLE_MOD instead
            Function.Call(Hash.SET_VEHICLE_LIGHTS, brokenCar, 2);

            return new ActiveScenario
            {
                Type = ScenarioType.BrokenDown,
                Vehicles = new List<Vehicle> { brokenCar },
                Peds = new List<Ped> { civilian },
                Location = pos,
                SpawnTime = Game.GameTime,
                DespawnTime = Game.GameTime + config.BrokenDownDuration,
                LocationName = location.Name
            };
        }

        private ActiveScenario SpawnCrashScenario(ScenarioLocation location)
        {
            Vector3 pos = location.Position;
            float heading = location.Heading;

            float angle = heading * (float)Math.PI / 180f;
            Vector3 forward = new Vector3((float)Math.Sin(angle), (float)Math.Cos(angle), 0);
            Vector3 right = new Vector3((float)Math.Cos(angle), -(float)Math.Sin(angle), 0);

            // Create first crashed vehicle
            Vehicle car1 = World.CreateVehicle(VehicleHash.Schafter2, pos);
            car1.Heading = heading;

            // Set color using native function
            Function.Call(Hash.SET_VEHICLE_COLOURS, car1, config.CrashVehicle1PrimaryColor, config.CrashVehicle1PrimaryColor);

            car1.EngineHealth = 250.0f;
            car1.BodyHealth = 500.0f;
            Function.Call(Hash.SET_VEHICLE_ENGINE_HEALTH, car1, 250.0f);
            Function.Call(Hash.SET_VEHICLE_BODY_HEALTH, car1, 500.0f);
            Function.Call(Hash.SET_VEHICLE_UNDRIVEABLE, car1, true);

            // Create second crashed vehicle
            Vector3 car2Pos = pos + forward * 4f + right * 3f;
            Vehicle car2 = World.CreateVehicle(VehicleHash.Oracle2, car2Pos);
            car2.Heading = heading + 45f;

            // Set color using native function
            Function.Call(Hash.SET_VEHICLE_COLOURS, car2, config.CrashVehicle2PrimaryColor, config.CrashVehicle2PrimaryColor);

            car2.EngineHealth = 300.0f;
            car2.BodyHealth = 450.0f;
            Function.Call(Hash.SET_VEHICLE_ENGINE_HEALTH, car2, 300.0f);
            Function.Call(Hash.SET_VEHICLE_BODY_HEALTH, car2, 450.0f);
            Function.Call(Hash.SET_VEHICLE_UNDRIVEABLE, car2, true);

            // Add damage
            Function.Call(Hash.SET_VEHICLE_DAMAGE, car1, 2f, 3f, 500f, 100f, 500f, 100f);
            Function.Call(Hash.SET_VEHICLE_DAMAGE, car2, 1.5f, 2.5f, 400f, 80f, 400f, 80f);

            // Create pedestrians
            Ped ped1 = World.CreatePed(PedHash.Business02AMY, pos + forward * 2f - right * 2f);
            Ped ped2 = World.CreatePed(PedHash.Business03AFY, pos + forward * 4f);

            ped1.Heading = heading + 90f;
            ped2.Heading = heading - 90f;

            // Make them face each other
            ped1.Task.LookAt(ped2);
            ped2.Task.LookAt(ped1);

            // Add scenarios
            Function.Call(Hash.TASK_START_SCENARIO_IN_PLACE, ped1, "WORLD_HUMAN_AA_SMOKE", 0, true);
            Function.Call(Hash.TASK_START_SCENARIO_IN_PLACE, ped2, "WORLD_HUMAN_STAND_MOBILE", 0, true);

            // Hazard lights
            Function.Call(Hash.SET_VEHICLE_LIGHTS, car1, 2);
            Function.Call(Hash.SET_VEHICLE_LIGHTS, car2, 2);

            return new ActiveScenario
            {
                Type = ScenarioType.Crash,
                Vehicles = new List<Vehicle> { car1, car2 },
                Peds = new List<Ped> { ped1, ped2 },
                Location = pos,
                SpawnTime = Game.GameTime,
                DespawnTime = Game.GameTime + config.CrashDuration,
                LocationName = location.Name
            };
        }
    }

    public class Config
    {
        public int TowTruckChance { get; private set; } = 25;
        public int BrokenDownChance { get; private set; } = 25;
        public int CrashChance { get; private set; } = 25;
        public int NothingChance { get; private set; } = 25;

        public float SpawnDistance { get; private set; } = 0.5f;
        public float DespawnDistance { get; private set; } = 0.7f;
        public float CheckRadius { get; private set; } = 5f;
        public int MaxConcurrentScenarios { get; private set; } = 3;

        public int TowTruckDuration { get; private set; } = 600000;
        public int BrokenDownDuration { get; private set; } = 600000;
        public int CrashDuration { get; private set; } = 600000;

        public int TowTruckPrimaryColor { get; private set; } = 88;
        public int TowTruckSecondaryColor { get; private set; } = 0;
        public int BrokenDownPrimaryColor { get; private set; } = 64;
        public int CrashVehicle1PrimaryColor { get; private set; } = 27;
        public int CrashVehicle2PrimaryColor { get; private set; } = 0;

        public Config(string path)
        {
            if (File.Exists(path))
            {
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";") || line.StartsWith("[")) continue;

                    var parts = line.Split('=');
                    if (parts.Length != 2) continue;

                    string key = parts[0].Trim();
                    string value = parts[1].Trim();

                    int intVal;
                    float floatVal;

                    switch (key)
                    {
                        case "TowTruck":
                            if (int.TryParse(value, out intVal)) TowTruckChance = intVal;
                            break;
                        case "BrokenDown":
                            if (int.TryParse(value, out intVal)) BrokenDownChance = intVal;
                            break;
                        case "Crash":
                            if (int.TryParse(value, out intVal)) CrashChance = intVal;
                            break;
                        case "Nothing":
                            if (int.TryParse(value, out intVal)) NothingChance = intVal;
                            break;
                        case "SpawnDistance":
                            if (float.TryParse(value, out floatVal)) SpawnDistance = floatVal;
                            break;
                        case "DespawnDistance":
                            if (float.TryParse(value, out floatVal)) DespawnDistance = floatVal;
                            break;
                        case "CheckRadius":
                            if (float.TryParse(value, out floatVal)) CheckRadius = floatVal;
                            break;
                        case "MaxConcurrentScenarios":
                            if (int.TryParse(value, out intVal)) MaxConcurrentScenarios = intVal;
                            break;
                        case "TowTruckDuration":
                            if (int.TryParse(value, out intVal)) TowTruckDuration = intVal;
                            break;
                        case "BrokenDownDuration":
                            if (int.TryParse(value, out intVal)) BrokenDownDuration = intVal;
                            break;
                        case "CrashDuration":
                            if (int.TryParse(value, out intVal)) CrashDuration = intVal;
                            break;
                        case "TowTruckPrimary":
                            if (int.TryParse(value, out intVal)) TowTruckPrimaryColor = intVal;
                            break;
                        case "TowTruckSecondary":
                            if (int.TryParse(value, out intVal)) TowTruckSecondaryColor = intVal;
                            break;
                        case "BrokenDownPrimary":
                            if (int.TryParse(value, out intVal)) BrokenDownPrimaryColor = intVal;
                            break;
                        case "CrashVehicle1Primary":
                            if (int.TryParse(value, out intVal)) CrashVehicle1PrimaryColor = intVal;
                            break;
                        case "CrashVehicle2Primary":
                            if (int.TryParse(value, out intVal)) CrashVehicle2PrimaryColor = intVal;
                            break;
                    }
                }
            }
        }
    }

    public class ScenarioLocation
    {
        public Vector3 Position { get; set; }
        public float Heading { get; set; }
        public string Name { get; set; }
        public int LastSpawnTime { get; set; }

        public ScenarioLocation(Vector3 pos, float heading, string name = "")
        {
            Position = pos;
            Heading = heading;
            Name = string.IsNullOrEmpty(name) ? $"Location at {pos}" : name;
            LastSpawnTime = 0;
        }
    }

    public class ActiveScenario
    {
        public ScenarioType Type { get; set; }
        public List<Vehicle> Vehicles { get; set; }
        public List<Ped> Peds { get; set; }
        public Vector3 Location { get; set; }
        public string LocationName { get; set; }
        public int SpawnTime { get; set; }
        public int DespawnTime { get; set; }

        public void Cleanup()
        {
            if (Vehicles != null)
            {
                foreach (var vehicle in Vehicles)
                {
                    if (vehicle != null && vehicle.Exists())
                    {
                        vehicle.Delete();
                    }
                }
            }

            if (Peds != null)
            {
                foreach (var ped in Peds)
                {
                    if (ped != null && ped.Exists())
                    {
                        ped.Delete();
                    }
                }
            }
        }
    }

    public enum ScenarioType
    {
        TowTruck,
        BrokenDown,
        Crash
    }
}
using Web.Api.Persistence.Models;

namespace Web.Api.Util {
    public class UserDirector(StatusChange statusChange) {

        private readonly Guid[] guidId = { new Guid("315a341a-caa0-4137-b903-1065a97df647"),
                                           new Guid("214a5c33-0694-4755-b69c-8d515dbe2091"),
                                           new Guid("fc210992-9d5e-4cf8-9af1-b15f42c61780"),
                                           new Guid("1016d28a-b54e-4cff-bb9c-020aa3f6c05b"),
                                           new Guid("db6f970a-4782-41cd-9375-c596d2eb1981"),
                                           new Guid("b14b80eb-fb12-4a3f-9090-b1b1695fcb23"),
                                           new Guid("b14b80eb-fb12-4a3f-9090-b1b1695fcb23"),
                                           new Guid("89e2d0bb-72fe-40c3-b2bb-0a4730af0adf"),
                                           new Guid("8f72548f-bcc6-4263-b8de-f0d319ced2f9"),
                                           new Guid("20d2be27-955c-4245-9296-f0bf1b4d1894"),
                                           new Guid("03c3664a-a4fc-4d5a-8c23-bd00384f6c7b"),

                                          };

        public User MakeAlexFarmerProfile() {
             return new UserBuilder(email: "AFarmer@email.com", first: "Alex", last: "Farmer", pass: "12345", userId: guidId[0])
                .AddList("Exercise", new Guid("8c227c61-c566-4991-b208-fd534d3d868b"))
                    .AddTask("Run", statusChange.PendingId, guidId[1], guidId[2])
                        .AddSubTask("Run", "Buy Shoes", guidId[3])
                        .AddSubTask("Run", "Go to park", guidId[4])
                    .AddTask("Buy Shoes", statusChange.PendingId, guidId[5], guidId[6])
                    .AddTask("Go to park", statusChange.PendingId, guidId[7], guidId[8])
                    .AddTask("Walk", statusChange.PendingId, guidId[9], guidId[10])
                        .AddNote("Take the dogs on the walk", new Guid("bf5e6489-9164-4078-bfd4-67ef9ed82b75"))
                        .AddSubTask("Walk", "Go to park", new Guid("90dd4508-eea7-435c-8390-2b7221ed6c44"))

                .AddList("Random", new Guid("aa4eabe4-1fe7-43b2-9eda-09cfe47b8c15"))
                .GetFinalUser();
        }

        public User MakeJessieHopkinsProfile() {
             return new UserBuilder(email: "JHopkins@email.com", first: "Jessie", last: "Hopkins", pass: "password", userId: new Guid("b631308c-a4d6-4bbd-a935-3b6a10d2d52d"))
                .AddOrphanTask("Cook", statusChange.PendingId, new Guid("1f889288-69cc-45eb-aa9b-acbb3088e7e4"), new Guid("adce69ef-46a7-4611-90b3-8ae034201165"))
                    .AddNote("spaghetti", new Guid("3b103965-0bb8-456a-abda-c22fd63a4f18"))
                    .AddNote("tacos", new Guid("a6caf954-21a9-4dda-9b77-532be0534c09"))

                .AddOrphanTask("Buy ingredients", statusChange.PendingId)
                .AddOrphanTask("Chop veggies", statusChange.PendingId)
                .AddOrphanTask("Wash dishes", statusChange.PendingId)

                .AddSubTaskForOrphan("Cook", "Buy ingredients")
                .AddSubTaskForOrphan("Cook", "Chop veggies")
                .AddSubTaskForOrphan("Cook", "Wash dishes")

                .GetFinalUser();
        }


        public User MakeAprilRiceProfile() {
             return new UserBuilder(email: "ARice@email.com", first: "April", last: "Rice", pass: "ARice", userId: new Guid("2157303f-4e90-4e43-82b0-ae93c44d85ed"))
                .AddList("Work")

                .AddOrphanTask("Clean",statusChange.PendingId)
                    .AddNote("Bathroom")
                .AddOrphanTask("Clean kitchen",statusChange.PendingId)
                .AddOrphanTask("Clean bathroom",statusChange.PendingId)

                .AddSubTaskForOrphan("Clean", "Clean kitchen")
                .AddSubTaskForOrphan("Clean", "Clean bathroom")
                
                .GetFinalUser();
        }

        
        public User MakeNikoLogamProfile() {
             return new UserBuilder(email: "Nlogan@email.com", first: "Niko", last: "Logan", pass: "abc", userId: new Guid("87c42ac5-cda9-4672-9fb9-3bd7c8d93363"))
                .AddList("New list")
                
                .GetFinalUser();
        }


        public User MakeChuckFinleyProfile() {
             return new UserBuilder(email: "chuck.finley@email.com", first: "Chuck", last: "Finley", pass: "abc", userId: new Guid("8051a558-6f25-409b-9823-d5f5603ee625"))
                .AddList("Mustang Project")
                    .AddTask("Test Drive", statusChange.PendingId)
                        .AddNote("Destination, the mall. Far enough to satisfy the test drive.")
                    
                    .AddTask("Fix radiator", statusChange.PendingId)
                    .AddTask("Add water", statusChange.PendingId)
                    .AddTask("Check oil", statusChange.PendingId)
                    .AddTask("Check tire pressure", statusChange.PendingId)
                    .AddTask("Replace Spark plugs", statusChange.PendingId)
                        .AddNote("The plugs were fouled from the last test run.")
                        .AddNote("Auto parts store has a set for $23.45 ou the door.")
                        .AddNote("Check the price of new distributor and plug wires while at the auto parts store.")

                    .AddSubTask("Test Drive", "Add water")
                    .AddSubTask("Test Drive", "Check oil")
                    .AddSubTask("Test Drive", "Check tire pressure")
                    .AddSubTask("Test Drive", "Replace Spark plugs")
                    .AddSubTask("Add water", "Fix radiator")

                .AddList("ArchiveItems")
                    .AddTask("Replace heads", statusChange.CompleteId)
                
                .GetFinalUser();
        }

    }
}

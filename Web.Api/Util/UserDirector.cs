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

        /*
        public User MakeAlexFarmerProfile() {
             return new UserBuilder(email: "AFarmer@email.com", first: "Alex", last: "Farmer", pass: "12345", userId: guidId[0])
                .AddList("Exercise", new Guid("8c227c61-c566-4991-b208-fd534d3d868b"))
                    .AddTask("Run", statusChange.PendingId, guidId[1], guidId[2])
                        .AddTask("Buy Shoes", statusChange.PendingId, guidId[5], guidId[6])                
                        .AddSubTask("Run", "Buy Shoes", guidId[3])
                        .AddTask("Go to park", statusChange.PendingId, guidId[7], guidId[8])
                        .AddSubTask("Run", "Go to park", guidId[4])
                    
                    
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
        }*/

        public User MakeAlexFarmerProfile() { 
            return new UserBuilder(email: "AFarmer@email.com", first: "Alex", last: "Farmer", pass: "12345", userId: guidId[0]).GetFinalUser(); 
        }

        public User MakeJessieHopkinsProfile()
        {
            return new UserBuilder(email: "JHopkins@email.com", first: "Jessie", last: "Hopkins", pass: "password", userId: new Guid("b631308c-a4d6-4bbd-a935-3b6a10d2d52d"))
                .AddOrphanTask(taskname: "Play sports", statusId: statusChange.PendingId, taskId: new Guid("6f5db359-e994-4f0b-8c79-0287da440a24"), taskItemStatusHistoryId: new Guid("c799916f-45b3-45b7-ba96-f24d042120fd"))
                    .AddNote(content: "Play sports for 1 hour", noteId: new Guid("c0be5fb0-cecf-48f7-82ce-89d9bb9a1356"))
                    .AddOrphanTask(taskname: "Basketball", statusId: statusChange.PendingId, taskId: new Guid("8ab01f57-682f-4a77-84c1-49b81e073fbe"), taskItemStatusHistoryId: new Guid("31b90f14-adc2-426b-8733-dad5659064bb"))
                    .AddSubTaskForOrphan(parent: "Play sports", child: "Basketball")
                .GetFinalUser();
        }

        public User MakeAprilRiceProfile()
        {
            return new UserBuilder(email: "ARice@email.com", first: "April", last: "Rice", pass: "ARice", userId: new Guid("2157303f-4e90-4e43-82b0-ae93c44d85ed"))
                .AddList(listname: "Shopping", listId: new Guid("1e4220df-eb3f-488a-9bb0-7e4ad078081e"))
                .AddList(listname: "School project", listId: new Guid("6c7dd6b3-0ee4-476d-885f-6a281c19a8bd"))
                    .AddTask(taskname: "Buy poster", statusId: statusChange.PendingId, taskId: new Guid("88f9c41a-9c5e-468b-9370-a7212a6a1f5a"), taskItemStatusHistoryId: new Guid("c13b279a-dc0a-4a42-a0df-1595f548c708"))
                        .AddNote(content: "The budget is $10", noteId: new Guid("f1b2f010-3f0e-49ea-84f5-d7b73fad41c7"))
                        .AddNote(content: "The poster has to be PINK", new Guid("3d9211db-e649-4b8b-8b6a-3ea3d671bc70"))
                        
                    .GetFinalUser();
        }

        public User MakeNikoLoganProfile()
        {
            return new UserBuilder(email: "Nlogan@email.com", first: "Niko", last: "Logan", pass: "abc", userId: new Guid("87c42ac5-cda9-4672-9fb9-3bd7c8d93363"))
                .AddList(listname: "Party planning", listId: new Guid("da604851-6dc0-40f3-bfc4-524cdf574f46"))
                .AddOrphanTask(taskname: "Homework", statusId: statusChange.PendingId, taskId: new Guid("6fbf7ace-0be3-4065-90c8-0dcb1eed94d5"), taskItemStatusHistoryId: new Guid("f03ee943-5882-440c-8783-97d93c20f871"))
                    .AddOrphanTask()    
                    .AddSubTaskForOrphan(parent: "Party planning", child: )
                .GetFinalUser();
        }

    }
}

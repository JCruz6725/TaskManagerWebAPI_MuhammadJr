using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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
        */

        public User MakeAlexFarmerProfile() { 
            return new UserBuilder(email: "AFarmer@email.com", first: "Alex", last: "Farmer", pass: "12345", userId: guidId[0]).GetFinalUser(); 
        }

        public User MakeJessieHopkinsProfile()
        {
            return new UserBuilder(email: "JHopkins@email.com", first: "Jessie", last: "Hopkins", pass: "password", userId: new Guid("b631308c-a4d6-4bbd-a935-3b6a10d2d52d"))
                .AddOrphanTask(taskname: "Play sports", statusId: statusChange.PendingId, priority: 1, taskId: new Guid("6f5db359-e994-4f0b-8c79-0287da440a24"), taskItemStatusHistoryId: new Guid("c799916f-45b3-45b7-ba96-f24d042120fd"))
                    .AddNote(content: "Play sports for 1 hour", noteId: new Guid("c0be5fb0-cecf-48f7-82ce-89d9bb9a1356"))
                    
                    .AddOrphanTask(taskname: "Basketball", statusId: statusChange.PendingId, priority: 4, taskId: new Guid("8ab01f57-682f-4a77-84c1-49b81e073fbe"), taskItemStatusHistoryId: new Guid("31b90f14-adc2-426b-8733-dad5659064bb"))
                    .LinkOrphanTasks(parent: "Play sports", child: "Basketball")
                
                .GetFinalUser();
        }

        public User MakeAprilRiceProfile()
        {
            return new UserBuilder(email: "ARice@email.com", first: "April", last: "Rice", pass: "ARice", userId: new Guid("2157303f-4e90-4e43-82b0-ae93c44d85ed"))
                .AddList(listname: "Shopping", listId: new Guid("1e4220df-eb3f-488a-9bb0-7e4ad078081e"))

                .AddList(listname: "School project", listId: new Guid("6c7dd6b3-0ee4-476d-885f-6a281c19a8bd"))
                    .AddTask(taskname: "Buy poster", statusId: statusChange.PendingId, priority: 3, taskId: new Guid("88f9c41a-9c5e-468b-9370-a7212a6a1f5a"), taskItemStatusHistoryId: new Guid("c13b279a-dc0a-4a42-a0df-1595f548c708"))
                        .AddNote(content: "The budget is $10", noteId: new Guid("f1b2f010-3f0e-49ea-84f5-d7b73fad41c7"))
                        .AddNote(content: "The poster has to be PINK", new Guid("3d9211db-e649-4b8b-8b6a-3ea3d671bc70"))
                
                .GetFinalUser();
        }

        public User MakeNikoLoganProfile()
        {
            return new UserBuilder(email: "Nlogan@email.com", first: "Niko", last: "Logan", pass: "abc", userId: new Guid("87c42ac5-cda9-4672-9fb9-3bd7c8d93363"))
                .AddList(listname: "Bucket list", listId: new Guid("da604851-6dc0-40f3-bfc4-524cdf574f46"))

                .AddOrphanTask(taskname: "Turn in homework", statusId: statusChange.PendingId, priority: 8, taskId: new Guid("6fbf7ace-0be3-4065-90c8-0dcb1eed94d5"), taskItemStatusHistoryId: new Guid("f03ee943-5882-440c-8783-97d93c20f871"))
                    .AddOrphanTask(taskname: "Algebra homework", statusId: statusChange.PendingId, priority: 10, taskId: new Guid("678cb3bb-0bf3-4388-86f2-c7a5dd7d40f7"), taskItemStatusHistoryId: new Guid("5ef60242-a310-47b4-835f-bfc20cc4c3c5")) 
                    .LinkOrphanTasks(parent: "Turn in homework", child: "Algebra homework")
                    
                    .AddOrphanTask(taskname: "Biology homework", statusId: statusChange.PendingId, priority: 9, taskId: new Guid("f289498b-44bc-455e-ae12-74da3fa4f4bb"), taskItemStatusHistoryId: new Guid("83467745-56cf-4825-be45-d9472f6e5d07"))
                    .LinkOrphanTasks(parent: "Turn in homework", child: "Biology homework")
                
                .GetFinalUser();
        }

        public User MakeChuckFinleyProfile()
        {
            return new UserBuilder(email: "chuck.finley@email.com", first: "Chuck", last: "Finley", pass: "abc", userId: new Guid("8051a558-6f25-409b-9823-d5f5603ee625"))
                .AddOrphanTask(taskname: "Cook dinner", statusId: statusChange.PendingId, priority: 15, taskId: new Guid("023af9db-2d82-4f2c-aa40-c393d38de31b"), taskItemStatusHistoryId: new Guid("80e93f32-a8c7-4da0-8d9b-f777ca6c093f"))
                .AddOrphanTask(taskname: "Make tacos", statusId: statusChange.PendingId, priority: 20, taskId: new Guid("2c8e1422-336f-4c2b-88e2-5f6f8cfb180a"), taskItemStatusHistoryId: new Guid("36832639-a393-4a00-ba93-19ed592c81d5"))
                .AddOrphanTask(taskname: "Buy ingredients", statusId: statusChange.CompleteId, priority: 23, taskId: new Guid("26e33b69-80de-431d-bb36-c83889d4c0f7"), taskItemStatusHistoryId: new Guid("5b030691-c1f4-4358-9499-e3240533a991"))
                .AddOrphanTask(taskname: "Get gas", statusId: statusChange.PendingId, priority: 25, taskId: new Guid("2528f71f-540d-4571-a47b-699182a15b34"), taskItemStatusHistoryId: new Guid("08103af9-ed2d-4761-8afc-97eaacdd13a9"))
                .AddOrphanTask(taskname: "Get money", statusId: statusChange.CompleteId, priority: 30, taskId: new Guid("3e8d4b54-8a52-48c9-83da-79abf1abcaf2"), taskItemStatusHistoryId: new Guid("cc6eb125-9e88-4c04-985e-d34727b743e3"))
                    .AddNote(content: "$40", noteId: new Guid("e43f8acc-c7e3-4c71-8b7e-1e48b375c3a1"))
                    
                .AddOrphanTask(taskname: "Get car keys", statusId: statusChange.PendingId, priority: 28, taskId: new Guid("5372c948-0953-4589-b856-117afe007bb6"), taskItemStatusHistoryId: new Guid("a706c4f7-8a61-4186-857d-f8825bbfce5b"))
                
                .LinkOrphanTasks(parent: "Cook dinner", child: "Make tacos")
                .LinkOrphanTasks(parent: "Make tacos", child: "Buy ingredients")
                .LinkOrphanTasks(parent: "Get gas", child: "Get money")
                .LinkOrphanTasks(parent: "Get gas", child: "Get car keys")

                .AddList(listname: "Exercise", listId: new Guid("c1a22ecb-424d-4429-a688-07cd8e816d8e"))
                    .AddTask(taskname: "Gym", statusId: statusChange.PendingId, priority: 40, taskId: new Guid("1a104a78-ceb3-4f1e-9fa0-af0eddfbe850"), taskItemStatusHistoryId: new Guid("5d7db210-cf97-4f35-a63e-89c976d29abf"))
                    .AddTask(taskname: "Running", statusId: statusChange.CompleteId, priority: 41, taskId: new Guid("589c54d2-4d30-42d7-9a8b-c935e82589e2"), taskItemStatusHistoryId: new Guid("b3ee215e-6a5d-45d7-8a20-e34508bd6b37"))
               

  

                 .AddList(listname: "Mustang Project", listId: new Guid ("d6802914-94aa-4e92-ac51-e6a3be307c9b"))
                    .AddTask(taskname: "Test Drive", statusId: statusChange.PendingId, priority: 40, taskId: new Guid("a0f9318d-8ae4-46f1-8f3b-aed8ae09665d"), taskItemStatusHistoryId: new Guid("c20048e5-7112-48e2-b64a-11ee7ad33bc9"))
                        .AddNote(content: "Destination, the mall. Far enough to satisfy the test drive.", noteId: new Guid("5b5a87dc-bbb9-4b44-899f-482313834d04"))
                    
                    .AddTask(taskname: "Fix radiator", statusId: statusChange.PendingId, priority: 41, taskId: new Guid("51f23db4-e69c-4fcb-84ca-0e8b169db0dd"), taskItemStatusHistoryId: new Guid("90193b7f-2ca0-4faa-ae9a-a0fc7faae885"))
                    .AddTask(taskname: "Add water", statusId: statusChange.PendingId, priority: 42, taskId: new Guid("f43cb68a-cc9b-42d9-8b3c-ca6cea270b54"), taskItemStatusHistoryId: new Guid("3dc115a6-1dea-4c86-aed2-7587b00a77d9"))
                    .AddTask(taskname: "Check oil", statusId: statusChange.PendingId, priority: 43, taskId: new Guid("24b6895a-1a00-4858-b6d7-0b876c8bcf1c"), taskItemStatusHistoryId: new Guid("f45da603-da3f-4626-ab42-94963e9a1998"))
                    .AddTask(taskname: "Check tire pressure", statusId: statusChange.PendingId, priority: 44, taskId: new Guid("2c95d5a2-938c-4f3a-aa25-70721acaf5b6"), taskItemStatusHistoryId: new Guid("e363ed27-9b6b-4922-8e77-5e96109e6022"))
                        .AddStatus(statusChange.CompleteId, new Guid("e363ed27-9b6b-4922-8e77-5e96109e6022"))                    
                    .AddTask(taskname: "Replace Spark plugs", statusId: statusChange.PendingId, priority: 45, taskId: new Guid("524f6219-9843-4752-b7d2-0370310272db"), taskItemStatusHistoryId: new Guid("48ed3625-c94a-42ac-a36e-b955445204a2"))
                        .AddNote("The plugs were fouled from the last test run.", new Guid("c05b2426-a930-42dd-a406-e91a74bf628b"))
                        .AddNote("Auto parts store has a set for $23.45 out the door.", new Guid("d1d73a46-ef17-420d-9343-d9dd42b69dfa"))
                        .AddNote("Check the price of new distributor and plug wires while at the auto parts store.", new Guid("d55cb6b5-03de-4dc4-9e41-aae6aac1a223"))

                    .LinkTasks("Test Drive", "Add water")
                    .LinkTasks("Add water", "Check oil")
                    .LinkTasks("Check oil", "Check tire pressure")
                    .LinkTasks("Check tire pressure", "Replace Spark plugs")
                    .LinkTasks("Replace Spark plugs", "Fix radiator")

                .GetFinalUser();
        }

        public User MakeIrenePetersonProfile()
        {
            return new UserBuilder(email: "irene.peterson@email.com", first: "Irene", last: "Peterson", pass: "secret", userId: new Guid("acba2ef3-bc1a-4484-8314-55ec5d951a4a"))
                .AddList(listname: "Party planning", listId: new Guid("9c615bc5-79c1-4bc5-9697-c7b62333f15d"))
                    .AddTask(taskname: "Surprise Jessie!", statusId: statusChange.PendingId, priority: 50, taskId: new Guid("3de3ba44-d578-4f82-890d-205d2c04cdcf"), taskItemStatusHistoryId: new Guid("d9d14a0d-1ff8-4cea-99ca-0181e63d8c7c"))
                    .AddTask(taskname: "Setup party", statusId: statusChange.CompleteId, priority: 55, taskId: new Guid("15cbf50a-3da9-4535-a506-130f422d37ba"), taskItemStatusHistoryId: new Guid("29da6f71-1d41-45a5-83d0-cd37a381dacf"))
                        .AddNote("Some note1", Guid.NewGuid())
                        .AddNote("Some note2", Guid.NewGuid())
                        .AddNote("Some note3", Guid.NewGuid())

                    .AddTask(taskname: "Cater food", statusId: statusChange.PendingId, priority: 60, taskId: new Guid("8f344f5b-368f-492a-9e85-ea2772784f5e"), taskItemStatusHistoryId: new Guid("966f8d35-e85e-42c2-aa55-9b7f5b8291e0"))
                    .AddTask(taskname: "Buy party decor", statusId: statusChange.CompleteId, priority: 60, taskId: new Guid("d6c1e8ad-cff0-493f-b9f8-57fabcfc1566"), taskItemStatusHistoryId: new Guid("908b838f-5a83-4fc9-8573-0188fda80b31"))
                    .AddTask(taskname: "Buy present", statusId: statusChange.CompleteId, priority: 62, taskId: new Guid("b1160ddc-4c8c-442c-afab-235e631054c9"), taskItemStatusHistoryId: new Guid("f2f00bfa-9c74-4032-bea7-8f43866d4035"))
                    
                    .LinkTasks(parent: "Surprise Jessie!", child: "Setup party")
                    .LinkTasks(parent: "Setup party", child: "Cater food")
                    .LinkTasks(parent: "Setup party", child: "Buy party decor")
                    .LinkTasks(parent: "Setup party", child: "Buy present")
                
                .GetFinalUser();
        }
    }
}

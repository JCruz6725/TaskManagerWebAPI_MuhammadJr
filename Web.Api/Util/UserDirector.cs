using Web.Api.Persistence.Models;

namespace Web.Api.Util {
    public class UserDirector(StatusChange statusChange) {
        public User MakeAlexFarmerProfile() {
             return new UserBuilder(email: "AFarmer@email.com", first: "Alex", last: "Farmer", pass: "12345")
                .AddList("Exercise")
                    .AddTask("Run", statusChange.PendingId)

                .AddList("Some list from FluentAPI")
                    .AddTask("FluentTask01", statusChange.PendingId)
                        .AddNote("SomeContent")
                        .AddNote("SomeOtherContent")

                    .AddTask("Run", statusChange.PendingId)
                    .AddTask("Buy Shoes", statusChange.PendingId)
                    .AddTask("Go to park", statusChange.PendingId)
                    .AddTask("Walk", statusChange.PendingId)
                        .AddNote("Take the dogs on the walk")

                    .AddSubTask("Run","Buy Shoes")
                    .AddSubTask("Run","Go to park")
                    .AddSubTask("Walk","Go to park")
                .AddList("Random")
                .GetFinalUser();
        }

        public User MakeJessieHopkinsProfile() {
             return new UserBuilder(email: "JHopkins@email.com", first: "Jessie", last: "Hopkins", pass: "password")
                .AddOrphanTask("Cook", statusChange.PendingId)
                    .AddNote("spaghetti")
                    .AddNote("tacos")

                .AddOrphanTask("Buy ingredients", statusChange.PendingId)
                .AddOrphanTask("Chop veggies", statusChange.PendingId)
                .AddOrphanTask("Wash dishes", statusChange.PendingId)

                .AddSubTaskForOrphan("Cook", "Buy ingredients")
                .AddSubTaskForOrphan("Cook", "Chop veggies")
                .AddSubTaskForOrphan("Cook", "Wash dishes")

                .GetFinalUser();
        }


        public User MakeAprilRiceProfile() {
             return new UserBuilder(email: "ARice@email.com", first: "April", last: "Rice", pass: "ARice")
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
             return new UserBuilder(email: "Nlogan@email.com", first: "Niko", last: "Logan", pass: "abc")
                .AddList("New list")
                
                .GetFinalUser();
        }


        public User MakeChuckFinleyProfile() {
             return new UserBuilder(email: "chuck.finley@email.com", first: "Chuck", last: "Finley", pass: "abc")
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

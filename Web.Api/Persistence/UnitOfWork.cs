using Web.Api.Persistence.Repositories;

namespace Web.Api.Persistence
{
    public class UnitOfWork
    {
        private readonly TaskManagerAppDBContext _context;      //calls the the scaffolded EF Core database (All repos share this _context)
        public UnitOfWork(TaskManagerAppDBContext context)     //constructor for UofW that sets the db context
        {
            _context = context;
        }

                                                            //private fields created to be to set the repositories to be callsed in property fields
        private UserRepo? _UserRepo;                           
        private TaskItemRepo? _TaskItemRepo;                 
        private ListRepo? _ListRepo;


                                                            //get priivate peoperty fields for the Repos
                                                            //beofore adding anything check if _Repo is null 
                                                            //add a new  instance of _Repo with db parameter
                                                            //finally return the created _Repo
        public UserRepo User                                 
        {
            get
            {
                if (_UserRepo is null)                      
                {
                    _UserRepo = new UserRepo(_context);   
                }
                return _UserRepo;                         
            }
        } 
        public TaskItemRepo TaskItem                        
        {
            get
            {
                if (_TaskItemRepo is null)                      
                {
                    _TaskItemRepo = new TaskItemRepo(_context);
                }
                return _TaskItemRepo;
            }
        }
        public ListRepo List                           
        {
            get
            {
                if (_ListRepo is null)
                {
                    _ListRepo = new ListRepo(_context);
                }
                return _ListRepo;
            }
        }
        public void SaveChanges()                              
        {
            _context.SaveChanges();
        }
    }
}

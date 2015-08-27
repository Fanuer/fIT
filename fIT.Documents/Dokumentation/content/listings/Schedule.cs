    // Definiert einen Trainingsplan
    public class Schedule: IEntity<int>
    {
        public Schedule(int id, string name = "", string userId = "", ICollection<Exercise> exercises = null)
        {
            this.Id = id;
            this.Name = name;
            this.UserID = userId;
            this.Exercises = exercises;
        }
		
        public Schedule(): this(-1){}
        
        // DB ID
        public int Id { get; set; }
        // DisplayName des Trainingsplans
        [Required]
        public string Name { get; set; }
        // Fremdschluessel zum Nutzer (per Namenskonvention) 
        public string UserID { get; set; }
        // Uebungen (per Namenskonvention) 
        public virtual ICollection<Exercise> Exercises { get; set; }
    }
}

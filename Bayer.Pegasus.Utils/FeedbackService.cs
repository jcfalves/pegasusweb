using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;

namespace Bayer.Pegasus.Utils
{
    public class FeedbackService
    {

        public FeedbackService()
        {
            Success = true;
            Errors = new ArrayList();
            Fields = new List<string>();
            Results = new Dictionary<string, object>();
        }


       

        public FeedbackService(bool success, ArrayList errors)
        {
            this.Success = success;
            this.Errors = errors;
            Results = new Dictionary<string, object>();
        }


        public void AddCustomError(string errorMessage)
        {
            Success = false;
            this.Errors.Add(errorMessage);

        }

        public void AddCustomError(string field, string errorMessage)
        {
            AddCustomError(errorMessage);
            Fields.Add(field);
        }


        public void Import(FeedbackService feedbackService)
        {
            foreach (var error in feedbackService.Errors) {
                this.Errors.Add(error);
            }

            foreach (var fields in feedbackService.Fields)
            {
                this.Fields.Add(fields);
            }

            if (this.Errors.Count > 0) {
                this.Success = false;
            }
        
        }


        public bool Success
        {
            get;
            set;
        }

        public ArrayList Errors
        {
            get;
            set;
        }

        public bool HasErrors {
            get {
                return !Success;
            }
            
        }


        public List<string> Fields
        {
            get;
            set;
        }

        public Dictionary<string, object> Results
        {
            get;
            set;
        }
    }
}

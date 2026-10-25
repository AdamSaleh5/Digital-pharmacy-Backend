using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Constants
{
    public static class AppMessages
    {
        public const string InvalidInput = "Check the correctness of the entered data.";
        public const string UnexpectedError = "An unexpected error occurred. Please try again later.";
        public const string InvalidCredentials = "Invalid email or password.";
        public const string SearchTypeRequired = "SearchType is required.";
        public const string InvalidSearchType = "The entered SearchType value is invalid. Use TradeName (1) or ScientificName (2).";
        public const string MedicationNotFound = "The requested medication was not found.";
        public const string CountryNotFound = "The requested Country was not found.";
        public const string PharmacieNotFound = "The requested Pharmacie was not found.";
        public const string MedicationUpdated = "Medication details were updated successfully.";
        public const string PharmacieUpdated = "Pharmacie were updated successfully.";
        public const string UnauthorizedAccess = "You do not have permission to perform this action.";
        public const string AuthenticationRequired = "A valid access token is required to access this resource.";
        public const string Success = "The process was completed successfully.";
    }
}

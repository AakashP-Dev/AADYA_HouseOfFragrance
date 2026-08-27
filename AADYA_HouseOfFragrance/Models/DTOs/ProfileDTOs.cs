namespace AADYA_HouseOfFragrance.Models.DTOs
{
    /// <summary>
    /// DTO for updating a user's profile information.
    /// </summary>
    public class UpdateProfileDto
    {
        /// <summary>
        /// The user's full name.
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// The user's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The user's phone number.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// The user's default delivery address.
        /// </summary>
        public string Address { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for changing a user's password.
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        /// The user's current password (required for verification).
        /// </summary>
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// The new password (must meet complexity requirements).
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// Confirmation of the new password (must match NewPassword).
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for uploading a user avatar image.
    /// </summary>
    public class AvatarUploadDto
    {
        /// <summary>
        /// The Base64-encoded image string of the avatar.
        /// </summary>
        public string AvatarBase64 { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating or updating a user address.
    /// </summary>
    public class AddressDto
    {
        /// <summary>
        /// The first line of the address (e.g., street number and name).
        /// </summary>
        public string AddressLine1 { get; set; } = string.Empty;

        /// <summary>
        /// The second line of the address (e.g., apartment, suite, building) – optional.
        /// </summary>
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// The city of the address.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// The state of the address.
        /// </summary>
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// The postal/pincode of the address (6 digits for Indian addresses).
        /// </summary>
        public string Pincode { get; set; } = string.Empty;

        /// <summary>
        /// The phone number associated with this address.
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Whether this address is the user's default shipping address.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// A label for the address (e.g., "Home", "Office", "Other"). Defaults to "Home".
        /// </summary>
        public string Label { get; set; } = "Home";
    }
}
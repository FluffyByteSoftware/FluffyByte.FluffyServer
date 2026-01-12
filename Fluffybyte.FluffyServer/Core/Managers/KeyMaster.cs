/*
 * (KeyMaster.cs)
 *------------------------------------------------------------
 * Created - Wednesday, January 7, 2026
 * Created by - Jacob Chacko
 *------------------------------------------------------------
 */

using System.Security.Cryptography;
using System.Text;
using FluffyByte.Debugger;

namespace Fluffybyte.FluffyServer.Core.Managers;

/// <summary>
/// Provides password hashing and verification services using PBKDF2 with SHA256.
/// </summary>
public static class KeyMaster
{
    private const string Name = "KeyMaster";

    // PBKDF2 configuration
    private const int SaltSizeBytes = 16;
    private const int HashSizeBytes = 32;
    private const int IterationCount = 10000; // Number of iterations for PBKDF2

    /// <summary>
    /// Hashes a password using PBKDF2-SHA256 with a randomly generated salt.
    /// Returns a combined string of "salt:hash" suitable for storage.
    /// </summary>
    /// <param name="password">The plaintext password to hash.</param>
    /// <returns>A string in format "salt:hash" for storage, or null if hashing fails.</returns>
    public static string? HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            Scribe.Warn($"{Name}: Attempted to hash null or empty password.");
            return null;
        }

        try
        {
            // Generate random salt
            byte[] salt = new byte[SaltSizeBytes];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with PBKDF2 using Rfc2898DeriveBytes.Pbkdf2
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                IterationCount,
                HashAlgorithmName.SHA256,
                HashSizeBytes);

            // Return as "base64salt:base64hash" for easy storage
            var saltString = Convert.ToBase64String(salt);
            var hashString = Convert.ToBase64String(hash);

            return $"{saltString}:{hashString}";
        }
        catch (Exception ex)
        {
            Scribe.Error($"{Name}: Exception hashing password", ex);
            return null;
        }
    }

    /// <summary>
    /// Verifies a plaintext password against a stored hash.
    /// </summary>
    /// <param name="providedPassword">The plaintext password to verify.</param>
    /// <param name="storedHash">The stored hash in the format "salt:hash".</param>
    /// <returns>True if the password matches the hash, false otherwise.</returns>
    public static bool VerifyPassword(string providedPassword, string storedHash)
    {
        if (string.IsNullOrEmpty(providedPassword) || string.IsNullOrEmpty(storedHash))
        {
            Scribe.Warn($"{Name}: Attempted to verify null or empty password/hash.");
            return false;
        }

        try
        {
            // Split the stored hash into salt and hash
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
            {
                Scribe.Warn($"{Name}: Invalid hash format (expected 'salt:hash').");
                return false;
            }

            var saltString = parts[0];
            var hashString = parts[1];

            // Decode salt and hash from base64
            byte[] salt = Convert.FromBase64String(saltString);
            byte[] storedHashBytes = Convert.FromBase64String(hashString);

            // Hash the provided password with the same salt using Rfc2898DeriveBytes.Pbkdf2
            var computedHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(providedPassword),
                salt,
                IterationCount,
                HashAlgorithmName.SHA256,
                HashSizeBytes);

            // Compare computed hash with stored hash (constant-time comparison)
            return ConstantTimeEquals(computedHash, storedHashBytes);
        }
        catch (Exception ex)
        {
            Scribe.Error($"{Name}: Exception verifying password", ex);
            return false;
        }
    }

    /// <summary>
    /// Performs constant-time byte array comparison to prevent timing attacks.
    /// </summary>
    private static bool ConstantTimeEquals(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;

        int result = 0;
        for (int i = 0; i < a.Length; i++)
        {
            result |= a[i] ^ b[i];
        }

        return result == 0;
    }
}

/*
 *------------------------------------------------------------
 * (KeyMaster.cs)
 * See License.txt for licensing information.
 *-----------------------------------------------------------
 */
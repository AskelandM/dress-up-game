using Blazored.LocalStorage;

public class LocalStorageOutfitRepository : IOutfitRepository
{
    private readonly ILocalStorageService _protectedLocalStore;

    public LocalStorageOutfitRepository(ILocalStorageService protectedLocalStore)
    {
        _protectedLocalStore = protectedLocalStore;
    }

    public async Task<List<string>> LoadOutfitsAsync()
    {
        try
        {
            var outfitsResult = await _protectedLocalStore.GetItemAsync<List<string>>("savedOutfits");
            return outfitsResult ?? new List<string>();
        }
        // Handle case where the savedOutfits list is corrupted or not in expected format
        catch (System.Text.Json.JsonException ex)
        {
            Console.WriteLine($"Error loading outfits: {ex.Message}");
            // Optionally clear the corrupted value
            await _protectedLocalStore.RemoveItemAsync("savedOutfits");
            await _protectedLocalStore.SetItemAsync("savedOutfits", new List<string>());
            return new List<string>();
        }
    }

    public async Task<List<string>> SaveOutfitAsync(string name, Outfit outfit)
    {
        var savedOutfits = await LoadOutfitsAsync();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Outfit name cannot be empty.");
            return savedOutfits;
        }
        // update list with new oufit name
        if (!savedOutfits.Contains(name))
            savedOutfits.Add(name);
        // save to local storage
        await _protectedLocalStore.SetItemAsync("savedOutfits", savedOutfits);
        await _protectedLocalStore.SetItemAsync(name, outfit);
        Console.WriteLine($"Saving outfit '{name}' with {outfit.Parts.Count} parts.");
        return savedOutfits;
    }
    public async Task<Outfit?> GetOutfitAsync(string name)
    {
        var outfitsResult = await _protectedLocalStore.GetItemAsync<Outfit>(name);
        if (outfitsResult != null) {
            Console.WriteLine($"Retrieved outfit '{name}' with {outfitsResult.Parts.Count} parts.");
            return outfitsResult;
        }
        else {
            Console.WriteLine($"Retrieving oufit {name} failed");
            return null;
        }
    }
    public async Task<List<string>> DeleteOutfitAsync(string name)
    {
        // (TODO?: ask for confirmation) then delete outfit currently selected in dropdown
        // no need to check if item exists in savedOutfits because it has to if it's in the dropdown 
        var savedOutfits = await LoadOutfitsAsync();
        savedOutfits.Remove(name);
        await _protectedLocalStore.SetItemAsync("savedOutfits", savedOutfits);
        await _protectedLocalStore.RemoveItemAsync(name);
        Console.WriteLine($"Deleted outfit '{name}'");
        return savedOutfits;
    }
}
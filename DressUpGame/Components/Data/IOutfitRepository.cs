public interface IOutfitRepository
{
    Task<List<string>> LoadOutfitsAsync();
    Task<List<string>> SaveOutfitAsync(string name, Outfit outfit); // returns savedOutfits list
    Task<Outfit?> GetOutfitAsync(string name);
    Task<List<string>> DeleteOutfitAsync(string name); // returns savedOutfits list
}
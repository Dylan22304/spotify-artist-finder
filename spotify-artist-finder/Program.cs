using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.Interfaces.Entities;
using System;

namespace spotify_artist_finder
{
    class Program
    {
        public static void Main()
        {
            //loop until the user is done
            while (true)
            {
                //output the instructions for using the program
                Console.WriteLine("Welcome to Spotify Easy Search! Here you can easily find new music related to artists you already enjoy!");
                Console.WriteLine("Please enter the name of the music artist which you would like to search for or press enter to quit:");

                //store the artists name
                string artistName = Console.ReadLine();

                //check for quitting
                if (artistName == "") break;

                //create a new object for searching
                ArtistLookup db = new();

                //find the entry in the database for the artist which the user is searching for
                IArtist original_artist = db.FindArtist(artistName);

                //get related artists to the one the user is searching for
                Dictionary<string, List<IArtist>> artists = db.FindRelatedArtists(original_artist);

                //loop until the user is done
                while (true)
                {
                    //select a certain relation type to view
                    Console.WriteLine("\nWhat type of relationship would you like to view for this artist?");
                    foreach (var relation_type in artists.Keys)
                    {
                        Console.WriteLine(relation_type);
                    }
                    string user_selection = Console.ReadLine();

                    if (user_selection == "") break;

                    //check if user made a valid selection
                    if (artists.TryGetValue(user_selection, out List<IArtist>? returned_list))
                    {
                        //loop until the user is done
                        while (true)
                        {
                            //select an artist to view their other music
                            Console.WriteLine("\nWhose music would you like to explore?");
                            int index = 1;
                            foreach (var artist in returned_list)
                            {
                                Console.WriteLine($"{index}: {artist}");
                                index++;
                            }
                            user_selection = Console.ReadLine();

                            if (user_selection == "") break;

                            //convert the selection into an integer for the index
                            int artist_num;
                            try
                            {
                                artist_num = Int32.Parse(user_selection);
                                Console.WriteLine("HERE");
                                artist_num--;

                                //check if user made a valid selection
                                if (artist_num < returned_list.Count && artist_num > 0)
                                {
                                    List<IWork> recommended_songs = db.SongsMadeBy(returned_list[artist_num], original_artist);
                                }
                                else
                                {
                                    Console.WriteLine("HERE AGAIN");
                                    Console.WriteLine("NOT A VALID NUMBER");
                                }
                            }
                            catch (FormatException e)
                            {
                                Console.WriteLine("NOT A VALID NUMBER");
                            }
                        }
                    }
                }
            }
        }
    }
}

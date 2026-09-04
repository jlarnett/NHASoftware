using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Entities.Game;
using NHA.Website.Software.Entities.Sponsors;

namespace NHA.Website.Software.Services.TestDataSeeding;

public static class TestDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dbContext);

        var animePages = dbContext.AnimePages ?? throw new InvalidOperationException("ApplicationDbContext.AnimePages is not configured.");
        var gamePages = dbContext.GamePages ?? throw new InvalidOperationException("ApplicationDbContext.GamePages is not configured.");
        var ads = dbContext.Ads ?? throw new InvalidOperationException("ApplicationDbContext.Ads is not configured.");

        var changesPending = false;

        if (!await animePages.AnyAsync(cancellationToken))
        {
            var seededAnimePages = CreateAnimePages().ToList();
            if (!seededAnimePages.Any(a => a.Featured))
            {
                var featuredAnime = seededAnimePages
                    .OrderByDescending(a => a.AnimeJikanScore ?? 0)
                    .First();
                featuredAnime.Featured = true;
            }

            await animePages.AddRangeAsync(seededAnimePages, cancellationToken);
            changesPending = true;
        }
        else if (!await animePages.AnyAsync(a => a.Featured, cancellationToken))
        {
            var featuredAnime = await animePages
                .OrderByDescending(a => a.AnimeJikanScore)
                .FirstOrDefaultAsync(cancellationToken);

            if (featuredAnime is not null)
            {
                featuredAnime.Featured = true;
                changesPending = true;
            }
        }

        if (!await gamePages.AnyAsync(cancellationToken))
        {
            var seededGamePages = CreateGamePages().ToList();
            if (!seededGamePages.Any(g => g.Featured))
            {
                var featuredGame = seededGamePages
                    .OrderByDescending(g => g.GameScore ?? 0)
                    .First();
                featuredGame.Featured = true;
            }

            await gamePages.AddRangeAsync(seededGamePages, cancellationToken);
            changesPending = true;
        }
        else if (!await gamePages.AnyAsync(g => g.Featured, cancellationToken))
        {
            var featuredGame = await gamePages
                .OrderByDescending(g => g.GameScore)
                .FirstOrDefaultAsync(cancellationToken);

            if (featuredGame is not null)
            {
                featuredGame.Featured = true;
                changesPending = true;
            }
        }

        if (!await ads.AnyAsync(cancellationToken))
        {
            await ads.AddRangeAsync(CreateSponsorAds(), cancellationToken);
            changesPending = true;
        }

        if (changesPending)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public static async Task SeedAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);

        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
        await SeedAsync(dbContext, cancellationToken);
    }

    private static IEnumerable<AnimePage> CreateAnimePages()
    {
        return
        [
            new AnimePage
            {
                AnimeName = "Cowboy Bebop",
                AnimeSummary = "Crime is timeless. By the year 2071, humanity has expanded across the galaxy, filling the surface of other planets with settlements like those on Earth. These new societies are plagued by murder, drug use, and theft, and intergalactic outlaws are hunted by a growing number of tough bounty hunters. Spike Spiegel and Jet Black pursue criminals throughout space to make a humble living. Beneath his goofy and aloof demeanor, Spike is haunted by the weight of his violent past. Meanwhile, Jet manages his own troubled memories while taking care of Spike and the Bebop, their ship. The duo is joined by the beautiful con artist Faye Valentine, odd child Edward Wong Hau Pepelu Tivrusky IV, and Ein, a bioengineered Welsh corgi. While developing bonds and working to catch a colorful cast of criminals, the Bebop crew's lives are disrupted by a menace from Spike's past. As a rival's maniacal plot continues to unravel, Spike must choose between life with his newfound family or revenge for his old wounds. [Written by MAL Rewrite]",
                AnimeBackground = "When Cowboy Bebop first aired in spring of 1998 on TV Tokyo, only episodes 2-3, 7-15, and 18 were broadcast, it was concluded with a recap special known as Yose Atsume Blues. This was due to anime censorship having increased following the big controversies over Evangelion, as a result most of the series was pulled from the air due to violent content. Satellite channel WOWOW picked up the series in the fall of that year and aired it in its entirety uncensored. Cowboy Bebop was not a ratings hit in Japan, but sold over 19,000 DVD units in the initial release run, and 81,000 overall. Protagonist Spike Spiegel won Best Male Character, and Megumi Hayashibara won Best Voice Actor for her role as Faye Valentine in the 1999 and 2000 Anime Grand Prix, respectively. Cowboy Bebop's biggest influence has been in the United States, where it premiered on Adult Swim in 2001 with many reruns since. The show's heavy Western influence struck a chord with American viewers, where it became a gateway drug to anime aimed at adult audiences.",
                UpVotes = 1,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/4/19644l.jpg",
                AnimeGenres = "Action;Award Winning;Sci-Fi",
                AnimeJikanScore = 8.75,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = "https://www.youtube-nocookie.com/embed/gY5nDXOtv_o?enablejsapi=1&wmode=opaque&autoplay=1",
                EpisodeCount = 26,
                Platforms = "Crunchyroll;Netflix;Tubi TV"
            },
            new AnimePage
            {
                AnimeName = "Cowboy Bebop: Tengoku no Tobira",
                AnimeSummary = "Another day, another bounty—such is the life of the often unlucky crew of the Bebop. However, this routine is interrupted when Faye, who is chasing a fairly worthless target on Mars, witnesses an oil tanker suddenly explode, causing mass hysteria. As casualties mount due to a strange disease spreading through the smoke from the blast, a whopping three hundred million woolong price is placed on the head of the supposed perpetrator. With lives at stake and a solution to their money problems in sight, the Bebop crew springs into action. Spike, Jet, Faye, and Edward, followed closely by Ein, split up to pursue different leads across Alba City. Through their individual investigations, they discover a cover-up scheme involving a pharmaceutical company, revealing a plot that reaches much further than the ragtag team of bounty hunters could have realized. [Written by MAL Rewrite]",
                AnimeBackground = string.Empty,
                UpVotes = 0,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/1439/93480l.jpg",
                AnimeGenres = "Action;Sci-Fi",
                AnimeJikanScore = 8.38,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = string.Empty,
                EpisodeCount = 1,
                Platforms = "Netflix"
            },
            new AnimePage
            {
                AnimeName = "Trigun",
                AnimeSummary = "Vash the Stampede is the man with a $$60,000,000,000 bounty on his head. The reason: he's a merciless villain who lays waste to all those that oppose him and flattens entire cities for fun, garnering him the title The Humanoid Typhoon. He leaves a trail of death and destruction wherever he goes, and anyone can count themselves dead if they so much as make eye contact—or so the rumors say. In actuality, Vash is a huge softie who claims to have never taken a life and avoids violence at all costs. With his crazy doughnut obsession and buffoonish attitude in tow, Vash traverses the wasteland of the planet Gunsmoke, all the while followed by two insurance agents, Meryl Stryfe and Milly Thompson, who attempt to minimize his impact on the public. But soon, their misadventures evolve into life-or-death situations as a group of legendary assassins are summoned to bring about suffering to the trio. Vash's agonizing past will be unraveled and his morality and principles pushed to the breaking point. [Written by MAL Rewrite]",
                AnimeBackground = "The Japanese release by Victor Entertainment has different openings relating to the specific episode it's played on. The initial Geneon Entertainment USA (then known as Pioneer) releases on VHS and DVD (singles, Signature Series, and box set) used only the first opening on each episode. This was due to the Japanese licensor providing them clean materials for only the first opening to put the English credits on. Geneon later fixed this mistake on their Limited Edition tin releases in 2005/2006, as well as on the Remix singles. Following Geneon USA's demise in late 2007, the show went out of print. When FUNimation Entertainment picked up the show in 2010 and released it, they repeated Geneon's mistake of using only the first opening on every episode. This mistake was later fixed in 2013 on the Anime Classics re-release.",
                UpVotes = 0,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/1130/120002l.jpg",
                AnimeGenres = "Action;Adventure;Sci-Fi",
                AnimeJikanScore = 8.22,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = "https://www.youtube-nocookie.com/embed/bJVyIXeUznY?enablejsapi=1&wmode=opaque&autoplay=1",
                EpisodeCount = 26,
                Platforms = "Crunchyroll;Netflix"
            },
            new AnimePage
            {
                AnimeName = "Witch Hunter Robin",
                AnimeSummary = "Though hidden away from the general public, Witches—those with supernatural powers—have always existed in human societies. Neither numerous nor inherently evil, Witches are nonetheless capable of creating disorder by misusing their powers for criminal means. The task of solving supernatural crimes falls outside of the jurisdiction of normal authorities and is instead handled by the Solomon organization. Having finished her training in Italy, Robin Sena transfers to Solomon's local Japanese branch, STNJ. Possessing powerful pyrokinetic abilities, she is herself a Witch, putting her at odds with STNJ's methods of dealing with rogue Witches. In particular, Robin opposes the use of an elixir called Orbo, which can weaken or even neutralize a Witch's powers. If Robin wants to find her place within the organization, she must find a way to navigate the internal politics of Solomon while also handling the threat of hostile Witches—but both seem to be dangerous for very different reasons. [Written by MAL Rewrite]",
                AnimeBackground = string.Empty,
                UpVotes = 0,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/10/19969l.jpg",
                AnimeGenres = "Action;Drama;Mystery;Supernatural",
                AnimeJikanScore = 7.23,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = "https://www.youtube-nocookie.com/embed/7UkaILjPk8M?enablejsapi=1&wmode=opaque&autoplay=1",
                EpisodeCount = 26,
                Platforms = "Crunchyroll"
            },
            new AnimePage
            {
                AnimeName = "Bouken Ou Beet",
                AnimeSummary = "It is the dark century and the people are suffering under the rule of the devil, Vandel, who is able to manipulate monsters. The Vandel Busters are a group of people who hunt these devils, and among them, the Zenon Squad is known to be the strongest busters on the continent. A young boy, Beet, dreams of joining the Zenon Squad. However, one day, as a result of Beet's fault, the Zenon squad was defeated by the devil, Beltose. The five dying busters sacrificed their life power into their five weapons, Saiga. After giving their weapons to Beet, they passed away. Years have passed since then and the young Vandel Buster, Beet, begins his adventure to carry out the Zenon Squad's will to put an end to the dark century.",
                AnimeBackground = string.Empty,
                UpVotes = 0,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/7/21569l.jpg",
                AnimeGenres = "Action;Adventure;Fantasy",
                AnimeJikanScore = 6.93,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = string.Empty,
                EpisodeCount = 52,
                Platforms = string.Empty
            },
            new AnimePage
            {
                AnimeName = "Hachimitsu to Clover",
                AnimeSummary = "Yuuta Takemoto, a sophomore at an arts college, shares a cheap apartment with two seniors—the eccentric Shinobu Morita, who keeps failing to graduate due to his absenteeism, and the sensible Takumi Mayama, who acts as a proper senior to Takemoto, often looking out for him. Takemoto had not given much thought to his future until one fine spring day, when he meets the endearing Hagumi Hanamoto and falls in love at first sight. Incredibly gifted in the arts, Hagumi enrolls in Takemoto's university and soon befriends the popular pottery student Ayumi Yamada. Ayumi is already well acquainted with the three flatmates and secretly harbors deep feelings for one of them. Hachimitsu to Clover is a heartwarming tale of youth, love, soul-searching, and self-discovery, intricately woven through the complex relationships between five dear friends. [Written by MAL Rewrite]",
                AnimeBackground = "Hachimitsu to Clover was the first anime to air on Fuji Television's noitaminA block.",
                UpVotes = 0,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/1301/133577l.jpg",
                AnimeGenres = "Comedy;Drama;Romance",
                AnimeJikanScore = 7.98,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = "https://www.youtube-nocookie.com/embed/6TN4a0kZuXg?enablejsapi=1&wmode=opaque&autoplay=1",
                EpisodeCount = 24,
                Platforms = string.Empty
            },
            new AnimePage
            {
                AnimeName = "Monster",
                AnimeSummary = "Dr. Kenzou Tenma, an elite neurosurgeon recently engaged to his hospital director's daughter, is well on his way to ascending the hospital hierarchy. That is until one night, a seemingly small event changes Dr. Tenma's life forever. While preparing to perform surgery on someone, he gets a call from the hospital director telling him to switch patients and instead perform life-saving brain surgery on a famous performer. His fellow doctors, fiancée, and the hospital director applaud his accomplishment; but because of the switch, a poor immigrant worker is dead, causing Dr. Tenma to have a crisis of conscience. So when a similar situation arises, Dr. Tenma stands his ground and chooses to perform surgery on the young boy Johan Liebert instead of the town's mayor. Unfortunately, this choice leads to serious ramifications for Dr. Tenma—losing his social standing being one of them. However, with the mysterious death of the director and two other doctors, Dr. Tenma's position is restored. With no evidence to convict him, he is released and goes on to attain the position of hospital director. Nine years later when Dr. Tenma saves the life of a criminal, his past comes back to haunt him—once again, he comes face to face with the monster he operated on. He must now embark on a quest of pursuit to make amends for the havoc spread by the one he saved. [Written by MAL Rewrite]",
                AnimeBackground = string.Empty,
                UpVotes = 0,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/10/18793l.jpg",
                AnimeGenres = "Drama;Mystery;Suspense",
                AnimeJikanScore = 8.89,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = string.Empty,
                EpisodeCount = 74,
                Platforms = string.Empty
            },
            new AnimePage
            {
                AnimeName = "Naruto",
                AnimeSummary = "Twelve years ago, a colossal demon fox terrorized the world. During the monster's attack on the Hidden Leaf Village, the Hokage—the village's leader and most powerful ninja—sacrifices himself to seal the beast inside a newborn, relieving civilization from destruction while dooming the baby to a lonely life. Now, after years of being shunned and bullied, Naruto Uzumaki pesters the village with elaborate pranks and vandalism. Despite these antics, he works hard to achieve his dream: to become the Hokage and earn the acknowledgement of those who have mistreated him for his entire life. Naruto joins Team 7, a ninja squad made up of two of his peers—prodigy Sasuke Uchiha and clever Sakura Haruno. Under the aloof Kakashi Hatake's leadership, Team 7 takes on a series of difficult missions, forcing its members to grow in strength and comradery despite their many differences. Naruto strives to stand out in his rivalry with Sasuke and earn the romantic affection of Sakura. But as the trio brush against danger and death, their tragic pasts threaten to tear them apart. [Written by MAL Rewrite]",
                AnimeBackground = "Naruto received numerous awards during its airing, including the Best Full-Length Animation Program Award in the third UStv Awards and the 38th Best Animated Show in IGN's Top 100 Animated Series. The anime was released on DVD in 16 volumes by VIZ Media from July 4, 2006, to September 22, 2009. The company rereleased it in eight volumes from October 6, 2009, to December 14, 2010. VIZ Media also made available a Blu-ray version in eight volumes from November 3, 2020, to October 18, 2022. The series adapts the first 27 volumes of the original manga.",
                UpVotes = 0,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/1141/142503l.jpg",
                AnimeGenres = "Action;Adventure;Fantasy",
                AnimeJikanScore = 8.02,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = string.Empty,
                EpisodeCount = 220,
                Platforms = "Crunchyroll;Netflix"
            },
            new AnimePage
            {
                AnimeName = "One Piece",
                AnimeSummary = "Barely surviving in a barrel after passing through a terrible whirlpool at sea, carefree Monkey D. Luffy ends up aboard a ship under attack by fearsome pirates. Despite being a naive-looking teenager, he is not to be underestimated. Unmatched in battle, Luffy is a pirate himself who resolutely pursues the coveted One Piece treasure and the King of the Pirates title that comes with it. The late King of the Pirates, Gol D. Roger, stirred up the world before his death by disclosing the whereabouts of his hoard of riches and daring everyone to obtain it. Ever since then, countless powerful pirates have sailed dangerous seas for the prized One Piece only to never return. Although Luffy lacks a crew and a proper ship, he is endowed with a superhuman ability and an unbreakable spirit that make him not only a formidable adversary but also an inspiration to many. As he faces numerous challenges with a big smile on his face, Luffy gathers one-of-a-kind companions to join him in his ambitious endeavor, together embracing perils and wonders on their once-in-a-lifetime adventure. [Written by MAL Rewrite]",
                AnimeBackground = "The anime had a hiatus from October 13, 2024, to April 6, 2025. The airing time was Wednesdays 19:00 between October 20, 1999 - March 2001. The airing time was changed to Sundays 19:30 between April 2001 - December 2004. The airing time was changed to Sundays 9:30 between October 6, 2006 - October 13, 2024. The airing time was again changed to Sundays 23:15 on April 6, 2025. Several anime-original arcs have been adapted into light novels, and the series has inspired 50+ video games as of 2023. In June 2004, One Piece was licensed in North America by 4Kids Entertainment, which partnered with Viz Media for home video distribution. As One Piece proved unsuitable for their target demographic, 4Kids Entertainment censored the show to meet their standards, and, in December 2006, they stopped its production. In April 2007, Funimation took over the series licensing, providing an uncut version that remained faithful to the original release. In Japan, the anime's first 574 episodes were released exclusively on DVD by Avex Pictures from February 21, 2001, to December 4, 2013. Blu-rays also became available with the DVDs starting on January 8, 2014. In North America, Viz Media released the anime on DVD between February 28, 2006, and June 26, 2007. Funimation has re-released and continued the series since May 27, 2008. From March 23, 2021, the DVDs were accompanied by Blu-rays as well.",
                UpVotes = 45,
                DownVotes = 4,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/1244/138851l.jpg",
                AnimeGenres = "Action;Adventure;Fantasy",
                AnimeJikanScore = 8.73,
                AnimeStatus = "Currently Airing",
                Featured = false,
                TrailerUrl = "https://www.youtube-nocookie.com/embed/-tviZNY6CSw?enablejsapi=1&wmode=opaque&autoplay=1",
                EpisodeCount = 1,
                Platforms = "Crunchyroll;Netflix;Shahid"
            },
            new AnimePage
            {
                AnimeName = "Tennis no Oujisama",
                AnimeSummary = "At the request of his father, tennis prodigy Ryouma Echizen has returned from America and is ready to take the Japanese tennis scene by storm. Aiming to become the best tennis player in the country, he enrolls in Seishun Academy—home to one of the best middle school tennis teams in Japan. After Ryouma catches the captain's eye, he finds himself playing for a spot on the starting lineup in the intra-school ranking matches despite only being a freshman. Due to his age, the rest of the Seishun Boys' Tennis Team are initially reluctant to accept him, but his skill and determination convinces them to let him in. Armed with their new super rookie, Seishun sets out to claim a spot in the National Tournament, hoping to take the coveted title for themselves. In order to do so, the team must qualify by playing through the Tokyo Prefectural and Kanto Regionals. Yet, the road ahead of them is shared by a plethora of strong schools, each playing tennis in unique ways for their own reasons. Ryouma and his teammates must learn to cooperate if they want to become the champions they aspire to be. [Written by MAL Rewrite]",
                AnimeBackground = "On April 24, 2007, Viz Media released the first DVD box set in the United States. An additional three box sets have been released since January 15, 2008. However, these four sets only contain 50 of the 178 episodes. On April 2, 2021, Funimation obtained licensing rights to the series and announced a new dub was in the works.",
                UpVotes = 0,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/6/21624l.jpg",
                AnimeGenres = "Sports",
                AnimeJikanScore = 7.84,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = string.Empty,
                EpisodeCount = 178,
                Platforms = "Crunchyroll"
            },
            new AnimePage
            {
                AnimeName = "Fullmetal Alchemist",
                AnimeSummary = "Edward Elric, a young, brilliant alchemist, has lost much in his twelve-year life: when he and his brother Alphonse try to resurrect their dead mother through the forbidden act of human transmutation, Edward loses his brother as well as two of his limbs. With his supreme alchemy skills, Edward binds Alphonse's soul to a large suit of armor. A year later, Edward, now promoted to the fullmetal alchemist of the state, embarks on a journey with his younger brother to obtain the Philosopher's Stone. The fabled mythical object is rumored to be capable of amplifying an alchemist's abilities by leaps and bounds, thus allowing them to override the fundamental law of alchemy: to gain something, an alchemist must sacrifice something of equal value. Edward hopes to draw into the military's resources to find the fabled stone and restore his and Alphonse's bodies to normal. However, the Elric brothers soon discover that there is more to the legendary stone than meets the eye, as they are led to the epicenter of a far darker battle than they could have ever imagined. [Written by MAL Rewrite]",
                AnimeBackground = "Fullmetal Alchemist won the TV Feature Award in the 9th Animation Kobe Awards and was one of the Jury Recommended Works in the 2004 Japan Media Arts Festival in the anime division. As the manga was still on-going at the time, the anime midway through diverged from the manga. This led to it having an anime-only ending, unlike Fullmetal Alchemist: Brotherhood which would air years later. On July 31, 2016, FUNimation Entertainment's license to the series expired.",
                UpVotes = 1,
                DownVotes = 0,
                AnimeImageUrl = "https://cdn.myanimelist.net/images/anime/10/75815l.jpg",
                AnimeGenres = "Action;Adventure;Award Winning;Drama;Fantasy",
                AnimeJikanScore = 8.12,
                AnimeStatus = "Finished Airing",
                Featured = false,
                TrailerUrl = string.Empty,
                EpisodeCount = 51,
                Platforms = "Crunchyroll;Netflix"
            },
        ];
    }

    private static IEnumerable<GamePage> CreateGamePages()
    {
        return
        [
            new GamePage
            {
                Name = "Grand Theft Auto V",
                Summary = "<p>Rockstar Games went bigger, since their previous installment of the series. You get the complicated and realistic world-building from Liberty City of GTA4 in the setting of lively and diverse Los Santos, from an old fan favorite GTA San Andreas. 561 different vehicles (including every transport you can operate) and the amount is rising with every update. <br /> Simultaneous storytelling from three unique perspectives: <br /> Follow Michael, ex-criminal living his life of leisure away from the past, Franklin, a kid that seeks the better future, and Trevor, the exact past Michael is trying to run away from. <br /> GTA Online will provide a lot of additional challenge even for the experienced players, coming fresh from the story mode. Now you will have other players around that can help you just as likely as ruin your mission. Every GTA mechanic up to date can be experienced by players through the unique customizable character, and community content paired with the leveling system tends to keep everyone busy and engaged.</p> <p>Español<br /> Rockstar Games se hizo más grande desde su entrega anterior de la serie. Obtienes la construcción del mundo complicada y realista de Liberty City de GTA4 en el escenario de Los Santos, un viejo favorito de los fans, GTA San Andreas. 561 vehículos diferentes (incluidos todos los transportes que puede operar) y la cantidad aumenta con cada actualización.<br /> Narración simultánea desde tres perspectivas únicas:<br /> Sigue a Michael, ex-criminal que vive su vida de ocio lejos del pasado, Franklin, un niño que busca un futuro mejor, y Trevor, el pasado exacto del que Michael está tratando de huir.<br /> GTA Online proporcionará muchos desafíos adicionales incluso para los jugadores experimentados, recién llegados del modo historia. Ahora tendrás otros jugadores cerca que pueden ayudarte con la misma probabilidad que arruinar tu misión. Los jugadores pueden experimentar todas las mecánicas de GTA actualizadas a través del personaje personalizable único, y el contenido de la comunidad combinado con el sistema de nivelación tiende a mantener a todos ocupados y comprometidos.</p>",
                ImageUrl = "https://media.rawg.io/media/games/20a/20aa03a10cda45239fe22d035c0ebe64.jpg",
                GameScore = 4.47,
                Status = string.Empty,
                Genres = "Action",
                UpVotes = 4,
                DownVotes = 1,
                Platforms = "PC;PlayStation 5;Xbox Series S/X;PlayStation 4;PlayStation 3;Xbox 360;Xbox One",
                Released = "2013-09-17",
                Featured = false,
                TrailerUrl = null
            },
            new GamePage
            {
                Name = "The Witcher 3: Wild Hunt",
                Summary = "<p>The third game in a series, it holds nothing back from the player. Open world adventures of the renowned monster slayer Geralt of Rivia are now even on a larger scale. Following the source material more accurately, this time Geralt is trying to find the child of the prophecy, Ciri while making a quick coin from various contracts on the side. Great attention to the world building above all creates an immersive story, where your decisions will shape the world around you.</p> <p>CD Project Red are infamous for the amount of work they put into their games, and it shows, because aside from classic third-person action RPG base game they provided 2 massive DLCs with unique questlines and 16 smaller DLCs, containing extra quests and items.</p> <p>Players praise the game for its atmosphere and a wide open world that finds the balance between fantasy elements and realistic and believable mechanics, and the game deserved numerous awards for every aspect of the game, from music to direction.</p>",
                ImageUrl = "https://media.rawg.io/media/games/618/618c2031a07bbff6b4f611f10b6bcdbc.jpg",
                GameScore = 4.64,
                Status = string.Empty,
                Genres = "Action;RPG",
                UpVotes = 2,
                DownVotes = 0,
                Platforms = "Xbox Series S/X;PlayStation 5;macOS;PlayStation 4;Nintendo Switch;PC;Xbox One",
                Released = "2015-05-18",
                Featured = true,
                TrailerUrl = null
            }
        ];
    }

    private static IEnumerable<SponsorAd> CreateSponsorAds()
    {
        return
        [
            new SponsorAd
            {
                ImageUrl = "https://cdn.myanimelist.net/images/anime/2/69665l.jpg",
                AdRedirectUrl = "https://myanimelist.net/",
                Views = 0
            },
            new SponsorAd
            {
                ImageUrl = "https://cdn.myanimelist.net/images/anime/2/69665l.jpg",
                AdRedirectUrl = "https://myanimelist.net/",
                Views = 0
            }
        ];
    }
}

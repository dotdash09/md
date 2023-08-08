import json
import sys
from ytmusicapi import YTMusic

def get_artist_albums(channelId, lang):
    """
    아티스트의 앨범 모두 가져오기
    :param channelId: 채널 아이디 (ex: UCvolP1xNN2maB52Tb1PkXzg)
    """    
    ytmusic = YTMusic("browser.json", language=lang)
    artist = ytmusic.get_artist(channelId)

    search_results = ytmusic.get_artist_albums(
        channelId, 
        artist["albums"]["params"])
    
    if not search_results:
        print("No search results found.")
    else:        
        json_string = json.dumps(search_results, ensure_ascii=False, indent=2)
        print(json_string)

get_artist_albums(sys.argv[1], sys.argv[2])
# get_artist_albums('UCbypb9u1bZaH7N2_h5cMLuw') # 자우림
# get_artist_albums('UCRw0x9_EfawqmgDI2IgQLLg') # 아델
# get_artist_albums('UCvolP1xNN2maB52Tb1PkXzg') # 장범준
# get_artist_albums('UCdFe4KkWwZ_twpo-UECR-Nw') # 마룬5
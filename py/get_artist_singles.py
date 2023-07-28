import json
import sys
from ytmusicapi import YTMusic

def get_artist_singles(channelId):
    """
    아티스트의 싱글앨범 모두 가져오기
    :param channelId: 채널 아이디 (ex: UCvolP1xNN2maB52Tb1PkXzg)
    """    
    # ytmusic = YTMusic("browser.json", language="en")
    ytmusic = YTMusic("browser.json", language="ko")
    artist = ytmusic.get_artist(channelId)
    
    search_results = ytmusic.get_artist_albums(
        channelId, 
        artist["singles"]["params"])
    
    if not search_results:
        print("No search results found.")
    else:
        json_string = json.dumps(search_results, ensure_ascii=False, indent=2)
        print(json_string)

get_artist_singles(sys.argv[1])
# get_artist_singles('UCZ0Aezmtk-S2l8A9Ln-2lKw') # 캘빈 해리스
# get_artist_singles('UCvolP1xNN2maB52Tb1PkXzg') # 장범준
# get_artist_singles('UCbypb9u1bZaH7N2_h5cMLuw') # 자우림
# get_artist_singles('UCdFe4KkWwZ_twpo-UECR-Nw') # 마룬5
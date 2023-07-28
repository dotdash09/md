import json
import sys
from ytmusicapi import YTMusic

def get_artist(channelId):
    """
    아티스트 검색
    :param channelId: 채널 아이디 (ex: UCvolP1xNN2maB52Tb1PkXzg)
    """
    # ytmusic = YTMusic("browser.json", language="en")
    ytmusic = YTMusic("browser.json", language="ko")
    search_results = ytmusic.get_artist(channelId)

    if not search_results:
        print("No search results found.")
    else:
        json_string = json.dumps(search_results, ensure_ascii=False, indent=2)
        print(json_string)

get_artist(sys.argv[1])        
# get_artist('UCbypb9u1bZaH7N2_h5cMLuw') # 자우림
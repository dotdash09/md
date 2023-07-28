import json
import sys
from ytmusicapi import YTMusic

def get_album_browse_id(playlistId):

    ytmusic = YTMusic("browser.json", language="en")
    search_results = ytmusic.get_album_browse_id(playlistId)

    if not search_results:
        print("No search results found.")
    else:
        print(search_results)

get_album_browse_id(sys.argv[1])              
# get_album_browse_id('OLAK5uy_m-cVOCYm25T_lGxrNfrcsIpu9czxkN6zI')              
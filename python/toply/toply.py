import sys
import meshio
from os.path import exists

# Description
# This script gets parameter: path to file or file name
# and try to convert it to .ply file

def toply(path):
    if exists(path):
        data = meshio.read(path)
        new_path = path + "_toply.ply"
        meshio.write_points_cells(new_path, data.points, data.cells, file_format="ply")
    else:
        print("File does not exist", file=sys.stderr)


def main():
    if len (sys.argv) > 1:
        path = sys.argv[1]
        toply(path)
    else:
        print("Expected name of file or path to file", file=sys.stderr)


if __name__ == '__main__':
    main()

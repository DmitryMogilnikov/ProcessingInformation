import pytest

from os.path import join

from src.utils import path_checker


@pytest.mark.parametrize("file,suffix", [("test1.docx", ".docx"), 
                                         ("test1.pdf",  ".pdf" ), 
                                         ("test1.html", ".html"), 
                                         ("test1.doc",  ".doc" ),
                                         ("test1.djvu", ".djvu")])
def test_check_path_with_correct_args_does_not_raise(file: str, suffix: str):
    path = join("./test/testcase_files/", file)
    try:
        path_checker.check_path(path, suffix)
    except Exception:
        pytest.fail()

@pytest.mark.parametrize("path", ["/fully/non/existing/path", 
                                  "./non_existent_path/with_file.ext"
                                  "./test/testcase_files/non_existing_file.ext",
                                  "./file_only.txt"])
def test_check_path_with_non_existing_pathes_raises_file_not_found(path: str):
    with pytest.raises(FileNotFoundError):
        path_checker.check_path(path, "")

@pytest.mark.parametrize("file,suffix", [("test1.docx", ".ext"  ), 
                                         ("test1.pdf",  ".txt"  ), 
                                         ("test1.html", ".htmls"), 
                                         ("test1.doc",  ".docx" ),
                                         ("test1.djvu", ".djv"  )])
def test_check_path_with_wrong_suffix_raises_type_error(file: str, suffix: str):
    path = join("./test/testcase_files/", file)
    with pytest.raises(TypeError):
        path_checker.check_path(path, suffix)
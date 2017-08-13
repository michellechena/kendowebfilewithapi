var selected = Cookies.get('selected');

function setSelectedNode(selected) {
    var treeview = $("#navigationTreeView").data("kendoTreeView");
    if (selected) {
        Cookies.remove('selected');
        var node = $("#navigationTreeView").find('li').eq(selected);
        treeview.select(node);
    }
}

function populateNavigationTree(data, FromSearchFolder) {
    
    //alert(Cookies.get('ChangedIdOfFolder') + selected);
    var newDataSource = new kendo.data.HierarchicalDataSource({ data: data });
    var navigationTreeView = $('#navigationTreeView').data('kendoTreeView');
    navigationTreeView.setDataSource(newDataSource);
    if (FromSearchFolder == "FromSearchFolder") {
        setSelectedNode("");
    }
    else {
        if (selected) {
            if (Cookies.get('ChangedIdOfFolder') == "0") {
                setSelectedNode("0");
            }
            else {
                setSelectedNode(selected);
            }
        }
        else {
            setSelectedNode("0");
        }
    }
}
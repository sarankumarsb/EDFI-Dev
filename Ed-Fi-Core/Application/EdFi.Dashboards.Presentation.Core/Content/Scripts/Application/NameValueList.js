// This object contains a name and an array of values.
var NameValuePair = function (name) {
    var self = this;
    self.Name = name;
    self.Values = new Array();

    // Adds a value to the value array.
    this.AddValue = function (value) {
        self.Values.push(value);
    };
};

// Contains the list of selected items from the watch list
var NameValueList = function () {
    // this is done because "this" has a different value inside of the
    // methods.
    var self = this;
    self.data = new Array();

    // Adds a new NameValuePair object to the list.
    this.Add = function (nvPair) {
        self.data.push(nvPair);
    };

    // Adds multiple NameValuePair objects to the list.
    this.AddRange = function (nvPairArray) {
        for (var rangeCount = 0; rangeCount < nvPairArray.length; rangeCount++) {
            self.Add(nvPairArray[rangeCount]);
        }
    };

    // Clears the NameValuePair array.
    this.Clear = function () {
        self.data = new Array();
    };

    // Determines if an item with a particular name currently exists in the
    // list.
    this.Contains = function (name) {
        for (var containsCount = 0; containsCount < self.Count() ; containsCount++) {
            if (self.Item(containsCount).Name === name) {
                return true;
            }
        }

        return false;
    };

    // Returns the current count of list items.
    this.Count = function () {
        return self.data.length;
    };

    // Gets a list item by its name.
    this.GetByName = function (name) {
        for (var nameCount = 0; nameCount < self.Count() ; nameCount++) {
            if (self.Item(nameCount).Name === name) {
                return self.Item(nameCount);
            }
        }

        return null;
    };

    // Gets a list item by its index.
    this.Item = function (index) {
        return self.data[index];
    };

    // Replaces a list item with another item.
    this.Replace = function (name, nvPair) {
        for (var replaceCount = 0; replaceCount < self.Count() ; replaceCount++) {
            if (self.data[replaceCount].Name === name) {
                self.data[replaceCount] = nvPair;
            }
        }
    };

    // Returns the lists internal array.
    this.ToArray = function () {
        return self.data;
    };
};
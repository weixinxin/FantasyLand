
function clone(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local new_table = {}
        lookup_table[object] = new_table
        for key, value in pairs(object) do
            new_table[_copy(key)] = _copy(value)
        end
        return setmetatable(new_table, getmetatable(object))
    end
    return _copy(object)
end

--Create an class.
function class(classname, super)
    local cls

    -- inherited from Lua Object
    if super then
        cls = clone(super);
        cls.super = super;
    else
        cls = {__constructor = function() end}
    end

    cls.__cname = classname
    cls.__index = cls

    function cls.new(...)
        local instance = setmetatable({}, cls);
        instance.class = cls;
        instance:__constructor(...);
        return instance
    end

    return cls
end

function isInherited(obj, classname)
	if obj == nil then
		return false;
	end
	
  if obj.__cname == classname then
    return true;
  else
    if obj.super ~= nil then
    end
  end
end

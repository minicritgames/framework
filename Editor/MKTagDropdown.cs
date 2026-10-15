using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Minikit.Editor
{
    public class MKTagDropdown : AdvancedDropdown
    {
        public class TagItem : AdvancedDropdownItem
        {
            public readonly string key;

            public TagItem(string _label, string _key) : base(_label)
            {
                key = _key;
                id = StringComparer.Ordinal.GetHashCode(_key ?? _label);
            }
        }

        private const string checkPrefix = "✓ ";

        private readonly HashSet<string> selected;
        private readonly bool multiSelect;
        private readonly Action<string> onSelected;


        public MKTagDropdown(IEnumerable<string> _selected, bool _multiSelect, Action<string> _onSelected)
            : base(new AdvancedDropdownState())
        {
            selected = new HashSet<string>(_selected ?? Array.Empty<string>(), StringComparer.Ordinal);
            multiSelect = _multiSelect;
            onSelected = _onSelected;
            minimumSize = new Vector2(300f, 340f);
        }


        public static void Show(Rect _rect, IEnumerable<string> _selected, bool _multiSelect, Action<string> _onSelected)
        {
            new MKTagDropdown(_selected, _multiSelect, _onSelected).Show(_rect);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Tags");

            if (!multiSelect)
            {
                root.AddChild(new TagItem("None", ""));
                root.AddSeparator();
            }

            IReadOnlyList<string> allTags = MKTagRegistry.GetAllTags();

            HashSet<string> groupKeys = new(StringComparer.Ordinal);
            foreach (string key in allTags)
            {
                for (int separatorIndex = key.LastIndexOf(MKTag.separator); separatorIndex > 0; separatorIndex = key.LastIndexOf(MKTag.separator, separatorIndex - 1))
                {
                    groupKeys.Add(key.Substring(0, separatorIndex));
                }
            }

            Dictionary<string, AdvancedDropdownItem> groups = new(StringComparer.Ordinal);
            foreach (string key in allTags)
            {
                int separatorIndex = key.LastIndexOf(MKTag.separator);
                string segment = separatorIndex > 0 ? key.Substring(separatorIndex + 1) : key;
                AdvancedDropdownItem parent = root;
                if (separatorIndex > 0
                    && groups.TryGetValue(key.Substring(0, separatorIndex), out AdvancedDropdownItem parentGroup))
                {
                    parent = parentGroup;
                }

                if (groupKeys.Contains(key))
                {
                    AdvancedDropdownItem group = new AdvancedDropdownItem(segment)
                    {
                        id = StringComparer.Ordinal.GetHashCode("group:" + key)
                    };
                    parent.AddChild(group);
                    groups.Add(key, group);

                    group.AddChild(CreateItem(key, key));
                    group.AddSeparator();
                }
                else
                {
                    parent.AddChild(CreateItem(key, key));
                }
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem _item)
        {
            if (_item is TagItem tagItem
                && tagItem.key != null)
            {
                onSelected?.Invoke(tagItem.key);
            }
        }

        private TagItem CreateItem(string _label, string _key)
        {
            return new TagItem(GetLabel(_label, _key), _key);
        }

        private string GetLabel(string _label, string _key)
        {
            return multiSelect && selected.Contains(_key) ? checkPrefix + _label : _label;
        }
    }
} // Minikit.Editor namespace

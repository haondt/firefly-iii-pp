import { TreeNode } from '../../models/TreeNode';

/**
 * Returns true if node is a descendant of parent
 */
export const isDescendantOf = function(parent: TreeNode, node: TreeNode) {
    if (parent.items === null || parent.items === undefined || parent.items.length === 0) {
      return false;
    }

    for(let child of parent.items) {
      if (node == child || isDescendantOf(child, node)) {
        return true;
      }
    }

    return false;
}

export const removeFromParent = function(data: TreeNode[], node: TreeNode) {
    if (data.length == 0) {
      return false;
    }

    for (let i=0; i < data.length; i++) {
      let ancestor = data[i];
      if (ancestor == node) {
        data.splice(i, 1);
        return true;
      }
      if (_removeFromParent(ancestor, node)) {
        return true;
      }
    }

    return false;
}

const _removeFromParent = function(ancestor: TreeNode, node: TreeNode) {
    if (ancestor.items === null
      || ancestor.items === undefined
      || ancestor.items.length === 0) {
        return false;
    }

    for(let i=0; i < ancestor.items.length; i++) {
      let child = ancestor.items[i];
      if (child == node) {
        ancestor.items.splice(i, 1);
        return true;
      }

      if (_removeFromParent(child, node)) {
        return true;
      }
    }

    return false;
}
